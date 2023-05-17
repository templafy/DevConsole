$targetPath = Join-Path -Path $PSScriptRoot -ChildPath ".."
$shimPath = Join-Path -Path $targetPath -ChildPath "shims/dev.ps1"
$aliasName = "dev"
$consolePath = Join-Path -Path $targetPath -ChildPath "DevConsole\bin\Release\net7.0\DevConsole.exe"

if ($IsMacOS -or $IsLinux) {
    $consolePath = Join-Path -Path $targetPath -ChildPath "DevConsole\bin\Release\net7.0\DevConsole"
}

New-Alias -Name $aliasName -Value $shimPath -Force

$scriptblock = {
    param($commandName, $wordToComplete, $cursorPosition)
    $command = $consolePath + " [suggest:$($cursorPosition)] ""$($wordToComplete)"""

    Invoke-Expression $command | ForEach-Object {
        [System.Management.Automation.CompletionResult]::new($_)
    }
}

Register-ArgumentCompleter -CommandName $aliasName -ScriptBlock $scriptblock -Native
