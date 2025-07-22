namespace MacUtilGUI.Views

open System.Windows.Input
open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open Avalonia.Interactivity
open MacUtilGUI.ViewModels
open MacUtilGUI.Models

type MainWindow() as this =
    inherit Window()

    do this.InitializeComponent()

    member private this.InitializeComponent() = AvaloniaXamlLoader.Load(this)

    member private this.OnScriptButtonClick(sender: obj, e: RoutedEventArgs) =
        match sender with
        | :? Button as button ->
            match button.Tag with
            | :? ScriptInfo as script ->
                match this.DataContext with
                | :? MainWindowViewModel as vm -> (vm.SelectScriptCommand :> ICommand).Execute(script)
                | _ -> ()
            | _ -> ()
        | _ -> ()
