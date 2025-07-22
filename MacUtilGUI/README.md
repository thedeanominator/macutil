# MacUtil GUI

A cross-platform GUI application built with F# and Avalonia UI to run the shell scripts from the MacUtil project.

## Features

- **Script Browser**: Browse and categorize scripts from the `scripts/` directory
- **Script Details**: View detailed information about each script including description, category, and file path
- **One-Click Execution**: Run scripts with a single button click
- **Cross-Platform**: Works on macOS, Windows, and Linux
- **Modern UI**: Built with Avalonia UI using the Fluent theme

## Project Structure

```
MacUtilGUI/
├── MacUtilGUI.fsproj          # Project file
├── App.axaml                  # Application XAML
├── Program.fs                 # Application entry point
├── Models/
│   ├── ScriptInfo.fs          # Script information model
│   └── ScriptCategory.fs      # Script category model
├── Services/
│   ├── TomlParser.fs          # TOML file parsing service
│   └── ScriptService.fs       # Script loading and execution service
├── ViewModels/
│   ├── ViewModelBase.fs       # Base view model class
│   └── MainWindowViewModel.fs # Main window view model
└── Views/
    ├── MainWindow.axaml       # Main window XAML
    └── MainWindow.axaml.fs    # Main window code-behind
```

## Prerequisites

- .NET 8.0 SDK or later
- F# support

## Building and Running

1. Navigate to the MacUtilGUI directory:
   ```bash
   cd MacUtilGUI
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

## How It Works

1. **Script Discovery**: The application scans the `../scripts/` directory for TOML configuration files and shell scripts
2. **TOML Parsing**: Uses the `tab_data.toml` files to organize scripts into categories with descriptions
3. **Dynamic Loading**: Scripts are organized by category and loaded dynamically from the file system
4. **Execution**: When a script is selected and "Run Script" is clicked, it executes the shell script in a new terminal process

## Configuration

The application reads configuration from:
- `scripts/tabs.toml` - Main configuration listing script directories
- `scripts/*/tab_data.toml` - Category-specific script configurations

## Dependencies

- **Avalonia**: Cross-platform .NET UI framework
- **Tomlyn**: TOML parsing library for .NET

## Extending

To add new script categories:
1. Create a new directory under `scripts/`
2. Add the directory name to `scripts/tabs.toml`
3. Create a `tab_data.toml` file in the new directory with script definitions
4. The GUI will automatically discover and display the new scripts

## Troubleshooting

- Ensure the `scripts/` directory is one level up from the executable location
- Check that shell scripts have execute permissions (`chmod +x script.sh`)
- Verify TOML files are properly formatted
