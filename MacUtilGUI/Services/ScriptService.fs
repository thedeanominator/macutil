namespace MacUtilGUI.Services

open System
open System.IO
open System.Diagnostics
open System.Reflection
open Tomlyn
open MacUtilGUI.Models

module ScriptService =

    let assembly = Assembly.GetExecutingAssembly()

    let getEmbeddedResource (resourcePath: string) : string option =
        try
            // How F# handles embedded resources:
            // 1. Resource names are case-sensitive.
            // 2. They use the format "Namespace.Folder.FileName" with folders separated by dots.
            // 3. Hyphens in the directories are converted to underscores in the resource name.
            // 4. Hyphens in the filename are kept as-is.
            //    For example, "script-common/common-script.sh" becomes "MacUtilGUI.scripts_common.common-script.sh".
            // 5. The namespace is typically the assembly name, so we prefix it with "MacUtilGUI.".

            let pathParts = resourcePath.Replace("\\", "/").Split('/')
            let convertedParts = 
                pathParts 
                |> Array.mapi (fun i part -> 
                    if i = pathParts.Length - 1 then 
                        // Last part is filename - keep hyphens
                        part
                    else 
                        // Directory parts - convert hyphens to underscores
                        part.Replace("-", "_")
                )
            
            let convertedPath = String.Join(".", convertedParts)
            let resourceName = sprintf "MacUtilGUI.%s" convertedPath
            
            printfn "DEBUG: Resource path '%s' -> '%s'" resourcePath resourceName

            use stream = assembly.GetManifestResourceStream(resourceName)

            if stream <> null then
                use reader = new StreamReader(stream)
                Some(reader.ReadToEnd())
            else
                printfn "Resource not found: %s" resourceName
                None
        with ex ->
            printfn "Error reading embedded resource %s: %s" resourcePath ex.Message
            None

    let listEmbeddedResources () =
        let resourceNames = assembly.GetManifestResourceNames()
        printfn "Available embedded resources:"

        for name in resourceNames do
            printfn "  %s" name

    let scriptsBasePath = ""

    let loadScriptsFromDirectory (directoryPath: string) : ScriptCategory list =
        let mutable categories = []

        try
            let tabDataPath = sprintf "%s/tab_data.toml" directoryPath

            match getEmbeddedResource tabDataPath with
            | Some content ->
                try
                    let tomlDoc = Toml.Parse(content)

                    // Parse the TOML as a dynamic table
                    if tomlDoc.Diagnostics.Count > 0 then
                        printfn "TOML parsing warnings/errors for %s:" tabDataPath

                        for diag in tomlDoc.Diagnostics do
                            printfn "  %s" (diag.ToString())

                    let table = tomlDoc.ToModel()

                    // Look for 'data' array in the table
                    if table.ContainsKey("data") then
                        match table.["data"] with
                        | :? Tomlyn.Model.TomlTableArray as dataArray ->
                            for dataGroup in dataArray do
                                if dataGroup.ContainsKey("name") && dataGroup.ContainsKey("entries") then
                                    let groupName = dataGroup.["name"].ToString()

                                    match dataGroup.["entries"] with
                                    | :? Tomlyn.Model.TomlTableArray as entriesArray ->
                                        let scripts =
                                            [ for entry in entriesArray do
                                                  if
                                                      entry.ContainsKey("name")
                                                      && entry.ContainsKey("description")
                                                      && entry.ContainsKey("script")
                                                  then
                                                      let name = entry.["name"].ToString()
                                                      let description = entry.["description"].ToString()
                                                      let script = entry.["script"].ToString()
                                                      let fullPath = sprintf "%s/%s" directoryPath script

                                                      yield
                                                          { Name = name
                                                            Description = description
                                                            Script = script
                                                            TaskList = "I"
                                                            Category = groupName
                                                            FullPath = fullPath } ]

                                        if not scripts.IsEmpty then
                                            let category = { Name = groupName; Scripts = scripts }
                                            categories <- category :: categories
                                    | _ -> ()
                        | _ -> ()
                with ex ->
                    printfn "Error parsing TOML file %s: %s" tabDataPath ex.Message
            | None ->
                printfn "ERROR: Embedded resource not found: %s" tabDataPath
                printfn "This should not happen if resources are properly embedded!"
        with ex ->
            printfn "Error loading scripts from %s: %s" directoryPath ex.Message

        categories |> List.rev

    let loadAllScripts () : ScriptCategory list =
        let mutable allCategories = []

        try
            // Quick check of embedded resources
            let resourceNames = assembly.GetManifestResourceNames()
            printfn "INFO: Found %d embedded resources including common-script.sh: %b" 
                resourceNames.Length 
                (resourceNames |> Array.exists (fun name -> name.EndsWith("common-script.sh")))

            let mainTabsPath = "tabs.toml"

            match getEmbeddedResource mainTabsPath with
            | Some content ->
                let tomlDoc = Toml.Parse(content)
                let table = tomlDoc.ToModel()

                if table.ContainsKey("directories") then
                    match table.["directories"] with
                    | :? Tomlyn.Model.TomlArray as dirArray ->
                        for dir in dirArray do
                            let directory = dir.ToString()
                            let categories = loadScriptsFromDirectory directory
                            allCategories <- allCategories @ categories
                    | _ -> ()
            | None ->
                printfn "ERROR: Main tabs.toml not found in embedded resources!"
                printfn "This should not happen if resources are properly embedded!"
        with ex ->
            printfn "Error loading scripts: %s" ex.Message

        allCategories

    let runScript (scriptInfo: ScriptInfo) : string =
        try
            // Get the script content from embedded resources
            match getEmbeddedResource scriptInfo.FullPath with
            | Some scriptContent ->
                // Check if script sources common-script.sh
                let needsCommonScript = scriptContent.Contains(". ../common-script.sh") || scriptContent.Contains(". ../../common-script.sh")
                
                let finalScriptContent = 
                    if needsCommonScript then
                        printfn "DEBUG: Script needs common-script.sh, checking embedded resources:"
                        listEmbeddedResources() // Show all available resources for debugging
                        
                        // Get common script content
                        match getEmbeddedResource "common-script.sh" with
                        | Some commonContent ->
                            printfn "DEBUG: Successfully found common-script.sh in embedded resources"
                            // Remove sourcing line and combine scripts
                            let cleanedScript = 
                                scriptContent
                                    .Replace(". ../common-script.sh", "")
                                    .Replace(". ../../common-script.sh", "")
                                    .Trim()
                            
                            // Combine: shebang + common functions + original script (without sourcing)
                            let shebang = "#!/bin/sh -e\n\n"
                            let commonFunctions = 
                                commonContent
                                    .Replace("#!/bin/sh -e", "")
                                    .Replace("# shellcheck disable=SC2034", "")
                                    .Trim()
                            
                            sprintf "%s# Embedded common script functions\n%s\n\n# Original script content\n%s" 
                                shebang commonFunctions cleanedScript
                        | None ->
                            printfn "Warning: common-script.sh not found in embedded resources, using original script"
                            scriptContent
                    else
                        scriptContent

                // Create a temporary file to execute the script
                let tempDir = Path.GetTempPath()
                let scriptFileName = Path.GetFileName(scriptInfo.Script) // Get just the filename, not the full path

                let tempFileName =
                    sprintf "%s_%s" (Guid.NewGuid().ToString("N").Substring(0, 8)) scriptFileName

                let tempFilePath = Path.Combine(tempDir, tempFileName)

                try
                    // Write script content to temporary file
                    File.WriteAllText(tempFilePath, finalScriptContent)

                    // Make the temporary file executable
                    let chmodStartInfo = ProcessStartInfo()
                    chmodStartInfo.FileName <- "/bin/chmod"
                    chmodStartInfo.Arguments <- sprintf "+x \"%s\"" tempFilePath
                    chmodStartInfo.UseShellExecute <- false
                    chmodStartInfo.CreateNoWindow <- true

                    let chmodProc = Process.Start(chmodStartInfo)

                    if chmodProc <> null then
                        chmodProc.WaitForExit()

                    // Execute the script
                    let startInfo = ProcessStartInfo()
                    startInfo.FileName <- "/bin/bash"
                    startInfo.Arguments <- sprintf "\"%s\"" tempFilePath
                    startInfo.UseShellExecute <- false
                    startInfo.CreateNoWindow <- true
                    startInfo.RedirectStandardOutput <- true
                    startInfo.RedirectStandardError <- true

                    let proc = Process.Start(startInfo)

                    if proc <> null then
                        let output = proc.StandardOutput.ReadToEnd()
                        let error = proc.StandardError.ReadToEnd()
                        proc.WaitForExit()

                        let fullOutput =
                            if String.IsNullOrEmpty(error) then
                                output
                            else
                                sprintf "%s\n--- ERRORS ---\n%s" output error

                        printfn "Executed script: %s (Exit Code: %d)" scriptInfo.Name proc.ExitCode
                        fullOutput
                    else
                        let errorMsg = sprintf "Failed to start script: %s" scriptInfo.Name
                        printfn "%s" errorMsg
                        errorMsg
                finally
                    // Clean up temporary file
                    if File.Exists(tempFilePath) then
                        try
                            File.Delete(tempFilePath)
                        with _ ->
                            () // Ignore cleanup errors
            | None ->
                let errorMsg =
                    sprintf "Script content not found in embedded resources: %s" scriptInfo.FullPath

                printfn "%s" errorMsg
                errorMsg
        with ex ->
            let errorMsg = sprintf "Error running script %s: %s" scriptInfo.Name ex.Message
            printfn "%s" errorMsg
            errorMsg
