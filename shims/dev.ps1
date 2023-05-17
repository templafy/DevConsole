#!/usr/bin/env pwsh
function AssertDotnetVersion {
    param(
        [int]$Major
    )

    $versions = (dotnet --list-sdks)
    [Array]$versionFound = $versions | ?{$_.StartsWith("$Major")}

    if ($versionFound.Length -lt 1) {
        dotnet --list-sdks
        throw ".NET $Major SDK not found. Install from https://aka.ms/dotnet-download"
    }
}
$scriptPath = $PSScriptRoot

# To debug change those variables to Debug and $false
$configuration = If ($args.Contains("[debug]")) {"Debug"} Else {"Release"}
$rebuild = $true

$targetPath = Join-Path -Path $scriptPath -ChildPath "..\DevConsole"
$consolePath = Join-Path -Path $targetPath -ChildPath "bin\$configuration\net7.0\DevConsole.exe"

if ($IsMacOS -or $IsLinux) {
    $consolePath = Join-Path -Path $targetPath -ChildPath "bin\$configuration\net7.0\DevConsole"
}

$updateCheckSaveFile = Join-Path -Path ([System.IO.Path]::GetTempPath()) -ChildPath "dev-console-update-check"
$lastUpdateCheck = ""
if (Test-Path $updateCheckSaveFile) {
    $lastUpdateCheck = Get-Content $updateCheckSaveFile
}

if ($lastUpdateCheck -le (Get-Date).AddHours(-12).ToString("o")) {
    Set-Content -Path $updateCheckSaveFile (Get-Date -Format "o")
    if (Test-Path $consolePath) {
        Write-Output "Checking for updates..."
        & $consolePath check-for-updates
    }
}

$buildSaveFile = Join-Path -Path ([System.IO.Path]::GetTempPath()) -ChildPath "dev-console-modified-$($configuration)"
$lastBuild = ""
if (Test-Path $buildSaveFile) {
    $lastBuild = Get-Content $buildSaveFile
}

$lastModified = Get-ChildItem -Path $targetPath -Exclude bin, obj | Get-ChildItem -File -Recurse | Sort-Object -Descending -Property LastWriteTimeUtc | Select-Object -ExpandProperty LastWriteTimeUtc -First 1

if ($rebuild -and ($lastBuild -ne $lastModified.ToString("o") -or !(Test-Path $consolePath))) {
    AssertDotnetVersion 7
    Write-Output "Change detected. Building..."
    dotnet build $targetPath --configuration $configuration | Out-Null
    if(!$?) {
        dotnet build $targetPath --configuration $configuration --interactive
        if (!$?) {
            Exit
        }
    }
    $lastModified = Get-ChildItem -Path $targetPath -Exclude bin, obj | Get-ChildItem -File -Recurse | Sort-Object -Descending -Property LastWriteTimeUtc | Select-Object -ExpandProperty LastWriteTimeUtc -First 1
    Set-Content -Path $buildSaveFile $lastModified.ToString("o")
}

& $consolePath $args
