[CmdletBinding()]
param(
    [string]$ProjectPath,
    [Parameter(Mandatory = $true)][string]$ExecuteMethod,
    [string[]]$AdditionalArgs = @()
)

$ErrorActionPreference = 'Stop'
if (-not $ProjectPath) { $ProjectPath = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path }
$editor = & (Join-Path $PSScriptRoot 'Find-UnityEditor.ps1')
$logDirectory = Join-Path $ProjectPath 'tools\unity\logs'
New-Item -ItemType Directory -Force -Path $logDirectory | Out-Null
$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$logPath = Join-Path $logDirectory "batch-$stamp.log"

& $editor -batchmode -nographics -quit -projectPath $ProjectPath -logFile $logPath -executeMethod $ExecuteMethod @AdditionalArgs
$exitCode = $LASTEXITCODE
Write-Output "Unity batch log: $logPath"
exit $exitCode
