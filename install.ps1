$shimFolder = Join-Path -Path $PSScriptRoot -ChildPath "shims"
$utilitiesFolder = Join-Path -Path $env:USERPROFILE -ChildPath ".devconsole"
$modulePath = Join-Path -Path $PSScriptRoot -ChildPath "DevConsoleModule/DevConsoleModule.psm1"

Write-Host "Checking PATH for shim folder: $shimFolder"
if ($env:Path -notlike "*$($shimFolder)*") {
    Write-Host "Adding shims to PATH..."
    $userPath = [Environment]::GetEnvironmentVariable("Path", [System.EnvironmentVariableTarget]::User)
    $newUserPath = $userPath + ";" + $shimFolder
    [Environment]::SetEnvironmentVariable("Path", $newUserPath, [System.EnvironmentVariableTarget]::User)
    $env:Path = [System.Environment]::GetEnvironmentVariable("Path",[System.EnvironmentVariableTarget]::Machine) + ";" + [System.Environment]::GetEnvironmentVariable("Path",[System.EnvironmentVariableTarget]::User) 
}

Write-Host "Checking PATH for utilities folder: $utilitiesFolder"
if ($env:Path -notlike "*$($utilitiesFolder)*") {
    Write-Host "Adding utilities to PATH..."
    $userPath = [Environment]::GetEnvironmentVariable("Path", [System.EnvironmentVariableTarget]::User)
    $newUserPath = $userPath + ";" + $utilitiesFolder
    [Environment]::SetEnvironmentVariable("Path", $newUserPath, [System.EnvironmentVariableTarget]::User)
    $env:Path = [System.Environment]::GetEnvironmentVariable("Path",[System.EnvironmentVariableTarget]::Machine) + ";" + [System.Environment]::GetEnvironmentVariable("Path",[System.EnvironmentVariableTarget]::User)
    
    if (-not (Test-Path $utilitiesFolder)) {
        Write-Host "Creating utilities folder..."
        New-Item -Path $utilitiesFolder -Type Directory -Force | Out-Null
    }
}

Write-Host "Checking profile for module import..."

if (!(Test-Path $PROFILE)) {
    Write-Host "Creating new profile..."
    New-Item -Path $PROFILE -ItemType File -Force | Out-Null
}

if (!(Select-String -Path $PROFILE "<# dev-console-import #>")) {
    Write-Host "Adding module import to $($PROFILE)..."
    Add-Content -Path $PROFILE -Value "<# dev-console-import #> Import-Module $($modulePath)"
}

Write-Host "Importing module..."

Import-Module $modulePath

Write-Host "Installation complete."

Write-Host "Welcome to dev!"
dev --help

