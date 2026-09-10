[CmdletBinding()]
param(
    [string]$Version = '6000.3.23f1'
)

$ErrorActionPreference = 'Stop'
$candidatePaths = @(
    "W:\Program Files\Unity\$Version\Editor\Unity.exe",
    "C:\Program Files\Unity\$Version\Editor\Unity.exe",
    "C:\Program Files\Unity Hub\Editor\$Version\Editor\Unity.exe"
)

$editor = $candidatePaths | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
if (-not $editor) {
    $unityCli = Get-Command unity.exe -ErrorAction SilentlyContinue
    if ($unityCli) {
        $installed = & $unityCli.Source --non-interactive editors --installed --verbose --json | ConvertFrom-Json
        $editor = $installed.data | Where-Object { $_.version -eq $Version } | Select-Object -ExpandProperty location -First 1
    }
}

if (-not $editor -or -not (Test-Path -LiteralPath $editor)) {
    throw "Unity editor $Version was not found. Install that exact version before running Unity validation."
}

Write-Output ([IO.Path]::GetFullPath($editor))
