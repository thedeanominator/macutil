namespace MacUtilGUI.ViewModels

open System.Collections.ObjectModel
open System.Windows.Input
open System.Threading.Tasks
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
    let mutable isScriptRunning: bool = false
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
            | Some script when not isScriptRunning ->
                isScriptRunning <- true
                scriptOutput <- "Starting script...\n"
                this.OnPropertyChanged("ScriptOutput")
                this.OnPropertyChanged("CanRunScript")
                this.OnPropertyChanged("IsScriptRunning")

                // Define output and error handlers
                let onOutput (line: string) =
                    Dispatcher.UIThread.InvokeAsync(fun () ->
                        scriptOutput <- scriptOutput + line + "\n"
                        this.OnPropertyChanged("ScriptOutput")
                    ) |> ignore

                let onError (line: string) =
                    Dispatcher.UIThread.InvokeAsync(fun () ->
                        scriptOutput <- scriptOutput + "[ERROR] " + line + "\n"
                        this.OnPropertyChanged("ScriptOutput")
                    ) |> ignore

                // Run script asynchronously with real-time output
                let scriptTask = ScriptService.runScript script onOutput onError
                scriptTask.ContinueWith(fun (task: Task<int>) ->
                    Dispatcher.UIThread.InvokeAsync(fun () ->
                        isScriptRunning <- false
                        scriptOutput <- scriptOutput + sprintf "\n=== Script completed with exit code: %d ===" task.Result
                        this.OnPropertyChanged("ScriptOutput")
                        this.OnPropertyChanged("CanRunScript")
                        this.OnPropertyChanged("IsScriptRunning")
                    ) |> ignore
                ) |> ignore
            | _ -> ())

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

    member _.CanRunScript = selectedScript.IsSome && not isScriptRunning

    member _.IsScriptRunning = isScriptRunning

    member _.SelectScriptCommand = selectScriptCommand

    member _.RunScriptCommand = runScriptCommand

    member _.Title = "MacUtil GUI - Script Runner"
