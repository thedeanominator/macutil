namespace MacUtilGUI.ViewModels

open System.Collections.ObjectModel
open System.Windows.Input
open Avalonia.Threading
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
    let mutable scriptOutput: string = ""
    let categories = ObservableCollection<ScriptCategory>()

    let selectScriptCommand =
        RelayCommand(fun parameter ->
            match parameter with
            | :? ScriptInfo as script ->
                selectedScript <- Some script
                scriptOutput <- "" // Clear previous output
                this.OnPropertyChanged("SelectedScript")
                this.OnPropertyChanged("ScriptOutput")
                this.OnPropertyChanged("CanRunScript")
                this.OnPropertyChanged("SelectedScriptName")
                this.OnPropertyChanged("SelectedScriptDescription")
                this.OnPropertyChanged("SelectedScriptCategory")
                this.OnPropertyChanged("SelectedScriptFile")
            | _ -> ())

    let runScriptCommand =
        RelayCommand(fun _ ->
            match selectedScript with
            | Some script ->
                scriptOutput <- "Running script..."
                this.OnPropertyChanged("ScriptOutput")

                // Run script in background to avoid blocking UI
                async {
                    let output = ScriptService.runScript script
                    // Update UI on the UI thread
                    Dispatcher.UIThread.InvokeAsync(fun () ->
                        scriptOutput <- output
                        this.OnPropertyChanged("ScriptOutput"))
                    |> ignore
                }
                |> Async.Start
            | None -> ())

    do
        // Load scripts on initialization
        let loadedCategories = ScriptService.loadAllScripts ()

        for category in loadedCategories do
            categories.Add(category)

    member _.Categories = categories

    member _.SelectedScript = selectedScript

    member _.ScriptOutput = scriptOutput

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
