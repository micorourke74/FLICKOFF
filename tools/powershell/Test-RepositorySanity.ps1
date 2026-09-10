[CmdletBinding()]
param(
    [string]$ProjectRoot
)

$ErrorActionPreference = 'Stop'
if (-not $ProjectRoot) { $ProjectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path }
$failures = [System.Collections.Generic.List[string]]::new()

function Require-Path([string]$RelativePath) {
    if (-not (Test-Path -LiteralPath (Join-Path $ProjectRoot $RelativePath))) {
        $failures.Add("Missing required path: $RelativePath")
    }
}

Push-Location $ProjectRoot
try {
    $root = (git rev-parse --show-toplevel 2>$null)
    if (-not $root -or [IO.Path]::GetFullPath($root) -ne [IO.Path]::GetFullPath($ProjectRoot)) {
        $failures.Add('Git root is not the requested project root.')
    }

    foreach ($path in @('Assets', 'Packages', 'ProjectSettings', 'Packages/manifest.json', 'Packages/packages-lock.json', 'ProjectSettings/ProjectVersion.txt', 'AGENTS.md', 'README.md', '.gitattributes')) {
        Require-Path $path
    }

    $tracked = @(git ls-files)
    $forbidden = @($tracked | Where-Object { $_ -match '(^|/)(Library|Temp|Obj|Logs|UserSettings|Build|Builds|Artifacts)(/|$)' })
    if ($forbidden.Count -gt 0) {
        $failures.Add("Generated Unity paths are tracked: $($forbidden -join ', ')")
    }

    $external = @($tracked | Where-Object { $_ -match '(^|/)(_external|reference/_external)(/|$)' })
    if ($external.Count -gt 0) {
        $failures.Add("External donor checkout paths are tracked: $($external -join ', ')")
    }

    try { Get-Content -Raw (Join-Path $ProjectRoot 'Packages/manifest.json') | ConvertFrom-Json | Out-Null } catch { $failures.Add("Packages/manifest.json is not valid JSON: $($_.Exception.Message)") }
    try { Get-Content -Raw (Join-Path $ProjectRoot 'Packages/packages-lock.json') | ConvertFrom-Json | Out-Null } catch { $failures.Add("Packages/packages-lock.json is not valid JSON: $($_.Exception.Message)") }

    $versionText = Get-Content -Raw (Join-Path $ProjectRoot 'ProjectSettings/ProjectVersion.txt')
    if ($versionText -notmatch 'm_EditorVersion:\s*6000\.3\.23f1') { $failures.Add('ProjectVersion.txt does not pin Unity 6000.3.23f1.') }

    $attributes = Get-Content -Raw (Join-Path $ProjectRoot '.gitattributes')
    if ($attributes -notmatch 'filter=lfs') { $failures.Add('.gitattributes does not define a Git LFS policy.') }

    $metaMissing = @(Get-ChildItem (Join-Path $ProjectRoot 'Assets') -Directory -Recurse | Where-Object { -not (Test-Path -LiteralPath ($_.FullName + '.meta')) })
    if ($metaMissing.Count -gt 0) { $failures.Add("Asset folders without paired .meta files: $($metaMissing.FullName -join ', ')") }

    git diff --check
    if ($LASTEXITCODE -ne 0) { $failures.Add('git diff --check failed.') }
}
finally {
    Pop-Location
}

if ($failures.Count -gt 0) {
    Write-Error (($failures | ForEach-Object { "FAIL: $_" }) -join [Environment]::NewLine)
    exit 1
}

Write-Output 'Repository sanity: PASS'
