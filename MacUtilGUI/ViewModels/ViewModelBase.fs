namespace MacUtilGUI.ViewModels

open System.ComponentModel

type ViewModelBase() =
    let propertyChanged = Event<PropertyChangedEventHandler, PropertyChangedEventArgs>()

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member _.PropertyChanged = propertyChanged.Publish

    member this.OnPropertyChanged(propertyName: string) =
        propertyChanged.Trigger(this, PropertyChangedEventArgs(propertyName))
