namespace MacUtilGUI.Services

open System
open System.IO
open System.Diagnostics
open System.Collections.Generic
open MacUtilGUI.Models

[<CLIMutable>]
type DirectoriesConfig = {
    directories: string[]
}

[<CLIMutable>]
type ScriptEntry = {
    name: string
    description: string
    script: string
    task_list: string
}

[<CLIMutable>]
type DataGroup = {
    name: string
    entries: ScriptEntry[]
}

[<CLIMutable>]
type TabData = {
    name: string
    data: DataGroup[]
}

module ScriptService =
    
    let scriptsBasePath = 
        let currentDir = Directory.GetCurrentDirectory()
        let scriptPath = Path.Combine(currentDir, "..", "scripts")
        let resolvedPath = Path.GetFullPath(scriptPath)
        printfn "Current directory: %s" currentDir
        printfn "Scripts path: %s" resolvedPath
        printfn "Scripts directory exists: %b" (Directory.Exists(resolvedPath))
        resolvedPath
    
    let convertToScriptInfo (entry: ScriptEntry) (category: string) (basePath: string) : ScriptInfo =
        let fullPath = Path.Combine(basePath, entry.script)
        {
            Name = entry.name
            Description = entry.description
            Script = entry.script
            TaskList = entry.task_list
            Category = category
            FullPath = fullPath
        }
    
    let loadScriptsFromDirectory (directoryPath: string) : ScriptCategory list =
        let mutable categories = []
        
        try
            let tabDataPath = Path.Combine(scriptsBasePath, directoryPath, "tab_data.toml")
            
            if File.Exists(tabDataPath) then
                try
                    let content = File.ReadAllText(tabDataPath)
                    let tabData = Tomlyn.Toml.ToModel<TabData>(content)
                    
                    for dataGroup in tabData.data do
                        let scripts = 
                            dataGroup.entries 
                            |> Array.map (fun entry -> 
                                convertToScriptInfo entry dataGroup.name (Path.Combine(scriptsBasePath, directoryPath)))
                            |> Array.toList
                        
                        let category = {
                            Name = dataGroup.name
                            Scripts = scripts
                        }
                        categories <- category :: categories
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
                let config = Tomlyn.Toml.ToModel<DirectoriesConfig>(content)
                
                for directory in config.directories do
                    let categories = loadScriptsFromDirectory directory
                    allCategories <- allCategories @ categories
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
