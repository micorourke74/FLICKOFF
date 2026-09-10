[CmdletBinding()]
param(
    [string]$ProjectRoot
)

$ErrorActionPreference = 'Stop'
if (-not $ProjectRoot) { $ProjectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path }
$candidateRoots = @(
    $env:UNITY_EDITOR_INSTALL_PATH,
    'W:\Program Files\Unity',
    'C:\Program Files\Unity',
    'C:\Program Files\Unity Hub\Editor'
) | Where-Object { $_ }

$mergeTool = $null
foreach ($root in $candidateRoots) {
    if (Test-Path -LiteralPath $root) {
        $mergeTool = Get-ChildItem -LiteralPath $root -Filter UnityYAMLMerge.exe -File -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($mergeTool) { break }
    }
}

if (-not $mergeTool) {
    throw 'UnityYAMLMerge.exe was not found. Open the project with Unity first or set UNITY_EDITOR_INSTALL_PATH.'
}

$quotedTool = '"' + $mergeTool.FullName.Replace('"', '\"') + '"'
Push-Location $ProjectRoot
try {
    git config merge.unityyamlmerge.name 'UnityYAMLMerge'
    git config merge.unityyamlmerge.driver "$quotedTool merge -p `"%O`" `"%A`" `"%B`""
    Write-Output "Configured local merge driver from $($mergeTool.FullName)"
}
finally {
    Pop-Location
}
