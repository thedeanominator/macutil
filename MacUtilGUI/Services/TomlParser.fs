namespace MacUtilGUI.Services

open System.IO
open Tomlyn
open MacUtilGUI.Models

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

module TomlParser =
    
    let parseTabDataFile (filePath: string) : TabData option =
        try
            if File.Exists(filePath) then
                let content = File.ReadAllText(filePath)
                let tabData = Toml.ToModel<TabData>(content)
                Some tabData
            else
                None
        with
        | ex ->
            printfn "Error parsing TOML file %s: %s" filePath ex.Message
            None

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
