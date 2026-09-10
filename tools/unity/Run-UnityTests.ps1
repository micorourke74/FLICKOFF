[CmdletBinding()]
param(
    [string]$ProjectPath,
    [ValidateSet('EditMode', 'PlayMode')][string]$TestPlatform = 'EditMode'
)

$ErrorActionPreference = 'Stop'
if (-not $ProjectPath) { $ProjectPath = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path }
$editor = & (Join-Path $PSScriptRoot 'Find-UnityEditor.ps1')
$unityCli = Get-Command unity.exe -ErrorAction SilentlyContinue
if (-not $unityCli) { throw 'The Unity CLI was not found on PATH.' }
$logDirectory = Join-Path $ProjectPath 'tools\unity\logs'
$resultDirectory = Join-Path $ProjectPath 'tools\unity\results'
New-Item -ItemType Directory -Force -Path $logDirectory, $resultDirectory | Out-Null
$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$logPath = Join-Path $logDirectory "tests-$TestPlatform-$stamp.log"
$resultPath = Join-Path $resultDirectory "results-$TestPlatform-$stamp.xml"

& $unityCli.Source --non-interactive --verbose test $ProjectPath --mode $TestPlatform --output $resultPath --editor-version 6000.3.23f1 --timeout 300 -- -nographics -logFile $logPath
$exitCode = $LASTEXITCODE
Write-Output "Unity test log: $logPath"
Write-Output "Unity test result: $resultPath"
exit $exitCode
