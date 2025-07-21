namespace MacUtilGUI.ViewModels

open System.Collections.ObjectModel
open System.Windows.Input
open MacUtilGUI.Models
open MacUtilGUI.Services

type RelayCommand(canExecute: obj -> bool, execute: obj -> unit) =
    let canExecuteChanged = Event<System.EventHandler, System.EventArgs>()
    
    interface ICommand with
        [<CLIEvent>]
        member _.CanExecuteChanged = canExecuteChanged.Publish
        member _.CanExecute(parameter) = canExecute parameter
        member _.Execute(parameter) = execute parameter
    
    new(execute: obj -> unit) = RelayCommand((fun _ -> true), execute)

type MainWindowViewModel() as this =
    inherit ViewModelBase()
    
    let mutable selectedScript: ScriptInfo option = None
    let categories = ObservableCollection<ScriptCategory>()
    
    let selectScriptCommand = RelayCommand(fun parameter ->
        match parameter with
        | :? ScriptInfo as script -> 
            selectedScript <- Some script
            this.OnPropertyChanged("SelectedScript")
            this.OnPropertyChanged("CanRunScript")
            this.OnPropertyChanged("SelectedScriptName")
            this.OnPropertyChanged("SelectedScriptDescription")
            this.OnPropertyChanged("SelectedScriptCategory")
            this.OnPropertyChanged("SelectedScriptFile")
        | _ -> ())
    
    let runScriptCommand = RelayCommand(fun _ -> 
        match selectedScript with
        | Some script -> ScriptService.runScript script
        | None -> ())
    
    do
        // Load scripts on initialization
        let loadedCategories = ScriptService.loadAllScripts()
        for category in loadedCategories do
            categories.Add(category)
    
    member _.Categories = categories
    
    member _.SelectedScript = selectedScript
    
    member _.SelectedScriptName = 
        match selectedScript with
        | Some script -> script.Name
        | None -> ""
    
    member _.SelectedScriptDescription = 
        match selectedScript with
        | Some script -> script.Description
        | None -> ""
    
    member _.SelectedScriptCategory = 
        match selectedScript with
        | Some script -> script.Category
        | None -> ""
    
    member _.SelectedScriptFile = 
        match selectedScript with
        | Some script -> script.Script
        | None -> ""
    
    member _.CanRunScript = selectedScript.IsSome
    
    member _.SelectScriptCommand = selectScriptCommand
    
    member _.RunScriptCommand = runScriptCommand
    
    member _.Title = "MacUtil GUI - Script Runner"
