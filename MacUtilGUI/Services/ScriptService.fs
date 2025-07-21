namespace MacUtilGUI.Services

open System
open System.IO
open System.Diagnostics
open Tomlyn
open MacUtilGUI.Models

module ScriptService =
    
    let scriptsBasePath = 
        let currentDir = Directory.GetCurrentDirectory()
        let scriptPath = Path.Combine(currentDir, "..", "scripts")
        let resolvedPath = Path.GetFullPath(scriptPath)
        printfn "Current directory: %s" currentDir
        printfn "Scripts path: %s" resolvedPath
        printfn "Scripts directory exists: %b" (Directory.Exists(resolvedPath))
        resolvedPath
    
    let loadScriptsFromDirectory (directoryPath: string) : ScriptCategory list =
        let mutable categories = []
        
        try
            let tabDataPath = Path.Combine(scriptsBasePath, directoryPath, "tab_data.toml")
            
            if File.Exists(tabDataPath) then
                try
                    let content = File.ReadAllText(tabDataPath)
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
                                            [
                                                for entry in entriesArray do
                                                    if entry.ContainsKey("name") && entry.ContainsKey("description") && entry.ContainsKey("script") then
                                                        let name = entry.["name"].ToString()
                                                        let description = entry.["description"].ToString()
                                                        let script = entry.["script"].ToString()
                                                        let fullPath = Path.Combine(scriptsBasePath, directoryPath, script)
                                                        
                                                        yield {
                                                            Name = name
                                                            Description = description
                                                            Script = script
                                                            TaskList = "I"
                                                            Category = groupName
                                                            FullPath = fullPath
                                                        }
                                            ]
                                        
                                        if not scripts.IsEmpty then
                                            let category = {
                                                Name = groupName
                                                Scripts = scripts
                                            }
                                            categories <- category :: categories
                                    | _ -> ()
                        | _ -> ()
                with
                | ex ->
                    printfn "Error parsing TOML file %s: %s" tabDataPath ex.Message
            else
                // If no tab_data.toml, scan for .sh files directly
                let directoryFullPath = Path.Combine(scriptsBasePath, directoryPath)
                if Directory.Exists(directoryFullPath) then
                    let shellFiles = Directory.GetFiles(directoryFullPath, "*.sh")
                    let scripts = 
                        shellFiles 
                        |> Array.map (fun filePath ->
                            let fileName = Path.GetFileNameWithoutExtension(filePath)
                            {
                                Name = fileName
                                Description = sprintf "Shell script: %s" fileName
                                Script = Path.GetFileName(filePath)
                                TaskList = "I"
                                Category = directoryPath
                                FullPath = filePath
                            })
                        |> Array.toList
                    
                    if not scripts.IsEmpty then
                        let category = {
                            Name = directoryPath
                            Scripts = scripts
                        }
                        categories <- category :: categories
        with
        | ex ->
            printfn "Error loading scripts from %s: %s" directoryPath ex.Message
        
        categories |> List.rev
    
    let loadAllScripts () : ScriptCategory list =
        let mutable allCategories = []
        
        try
            let mainTabsPath = Path.Combine(scriptsBasePath, "tabs.toml")
            if File.Exists(mainTabsPath) then
                let content = File.ReadAllText(mainTabsPath)
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
            else
                // Fallback: scan known directories
                let knownDirectories = ["applications-setup"; "system-setup"]
                for directory in knownDirectories do
                    let categories = loadScriptsFromDirectory directory
                    allCategories <- allCategories @ categories
        with
        | ex ->
            printfn "Error loading scripts: %s" ex.Message
        
        allCategories
    
    let runScript (scriptInfo: ScriptInfo) : unit =
        try
            let startInfo = ProcessStartInfo()
            startInfo.FileName <- "/bin/bash"
            startInfo.Arguments <- sprintf "-c \"cd '%s' && chmod +x '%s' && ./'%s'\"" 
                                           (Path.GetDirectoryName(scriptInfo.FullPath))
                                           (Path.GetFileName(scriptInfo.FullPath))
                                           (Path.GetFileName(scriptInfo.FullPath))
            startInfo.UseShellExecute <- true
            startInfo.CreateNoWindow <- false
            
            let proc = Process.Start(startInfo)
            if proc <> null then
                printfn "Started script: %s" scriptInfo.Name
            else
                printfn "Failed to start script: %s" scriptInfo.Name
        with
        | ex ->
            printfn "Error running script %s: %s" scriptInfo.Name ex.Message
