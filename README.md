# DevConsole template

Welcome to the Templafy DevConsole template. This was extracted from the internal Templafy DevConsole project to provide a template for others to get started creating their own DevConsole. To use this project, simply fork or copy this repository and start developing your own version.

The DevConsole is central to local development in Templafy. We use it to distribute any automation between our engineers. We built it in C# since that is the language we are most comfortable in and we really wanted the project to be as hackable as possible.
Our update mechanism is simply keeping this in source form in a Git repository and using shims to pull changes and rebuild if anything has changed. This allows consumers to use Git to test experiment branches from colleagues and have their own local changes running.

This template is a trimmed down version of the internal version but still provide enough commands to give an idea how to expand it. Our usage is based on an Azure and Azure DevOps environment. We decided to leave commands related to our main flows in, but feel free to discard these commands if you use different technology.

## Technology

This project is built around a few libraries:

- [System.CommandLine](https://www.nuget.org/packages/System.CommandLine) - Microsoft command line library in development that provides basic command framework and tab completing through directives.
- [Jab](https://github.com/pakrym/jab) - Source Generator based IoC container. We migrated from Microsoft dependency injection to speed up application launch.
- [Sharprompt](https://github.com/shibayan/Sharprompt) / [Spectre.Console](https://github.com/spectreconsole/spectre.console) - Make our CLIs and prompts look much better.

## Getting Started

When you clone or fork this repository, the first thing you should do is change the values in `DevConsole/DevConsoleConfig`. If you do not use Azure DevOps, some of the commands like creating pull requests and branches will need some adjustment, but we left them in to demonstate how it works for us.

## Installation

Requires [the .NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0).

### PowerShell

- Run from PowerShell / PowerShell Core:
```pwsh
./DevConsole/install.ps1
```

### Bash

Note: Bash tab completion is still not implemented.

- Install PowerShell Core
- Run from bash:
```bash
./DevConsole/install.bash
. ~/.bashrc
```

### zsh

Note: Zsh tab completion is still not implemented.

- Install PowerShell Core
- Run from bash:
```zsh
./DevConsole/install.zsh
. ~/.zshrc
```

## Updating

`dev` is built from source so pulling the `DevConsole` repository is the only required step.
