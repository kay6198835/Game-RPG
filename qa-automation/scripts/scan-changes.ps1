<#
.SYNOPSIS
  Change-impact scan: what landed on the sprint branch between two commits, which systems it touched,
  which QA case groups it affects, and the interaction risks to re-check.

.OUTPUTS
  <OutDir>/weekly-change-impact.md and <OutDir>/change-impact.json. Writes the list of systems that need a
  deep (P2) run to the pipeline as the 'deep' array of the JSON.

.EXAMPLE
  powershell -File scan-changes.ps1 -Repo D:\Fork\Game-RPG -Base origin/sprint-19 -Head origin/sprint-20 -OutDir .\out
#>
param(
    [Parameter(Mandatory = $true)][string]$Repo,
    [Parameter(Mandatory = $true)][string]$Base,
    [Parameter(Mandatory = $true)][string]$Head,
    [Parameter(Mandatory = $true)][string]$OutDir,
    [string]$MapFile = '',
    [string]$CatalogFile = ''
)
$ErrorActionPreference = 'Stop'
# Defaults resolved here: $PSScriptRoot is empty inside a param block under Windows PowerShell 5.1.
$qaRoot = Split-Path -Parent $PSScriptRoot
if (-not $MapFile) { $MapFile = Join-Path $qaRoot 'config/impact-map.json' }
if (-not $CatalogFile) { $CatalogFile = Join-Path $qaRoot 'cases/case-catalog.md' }
$utf8 = New-Object System.Text.UTF8Encoding $false

function Git { param([string[]]$GitArgs) $out = & git.exe -C $Repo @GitArgs 2>&1; if ($LASTEXITCODE -ne 0) { throw "git $($GitArgs -join ' ') failed: $out" }; return $out }

$map = Get-Content -Raw -Encoding UTF8 $MapFile | ConvertFrom-Json
$baseSha = (Git @('rev-parse', '--short=10', $Base)).Trim()
$headSha = (Git @('rev-parse', '--short=10', $Head)).Trim()

# --- commits and files ------------------------------------------------------------------------------
$sep = [char]0x1f
$log = Git @('log', '--no-merges', "--format=@@%h$sep%ad$sep%an$sep%s", '--date=short', '--name-status', '-M', "$Base..$Head")
$commits = New-Object System.Collections.ArrayList
$current = $null
foreach ($line in $log) {
    if ([string]::IsNullOrWhiteSpace($line)) { continue }
    if ($line.StartsWith('@@')) {
        $p = $line.Substring(2).Split($sep)
        $current = [ordered]@{ sha = $p[0]; date = $p[1]; author = $p[2]; subject = $p[3]; files = New-Object System.Collections.ArrayList }
        [void]$commits.Add($current)
        continue
    }
    $parts = $line -split "`t"
    $status = $parts[0].Substring(0, 1)
    $path = $parts[$parts.Length - 1]
    [void]$current.files.Add([ordered]@{ status = $status; path = $path })
}
$mergeCount = @(Git @('rev-list', '--merges', "$Base..$Head")).Where({ $_ }).Count

# --- classify ---------------------------------------------------------------------------------------
function Classify([string]$path) {
    foreach ($rule in $map.rules) {
        if ($path -match "(?i)$($rule.pattern)") { return $rule }
    }
    return $null
}

$fileState = @{}   # path -> last status letter
foreach ($c in $commits) { foreach ($f in $c.files) { $fileState[$f.path] = $f.status } }

$bySystem = @{}
$labels = @{}
$risks = New-Object System.Collections.ArrayList
$riskSeen = @{}
foreach ($path in ($fileState.Keys | Sort-Object)) {
    $rule = Classify $path
    foreach ($sys in $rule.systems) {
        if (-not $bySystem.ContainsKey($sys)) { $bySystem[$sys] = New-Object System.Collections.ArrayList }
        [void]$bySystem[$sys].Add($path)
    }
    $labels[$path] = $rule.label
    foreach ($risk in $map.risks) {
        if ($path -match "(?i)$($risk.pattern)") {
            $key = $risk.pattern
            if (-not $riskSeen.ContainsKey($key)) {
                $riskSeen[$key] = [ordered]@{ why = $risk.why; alsoCheck = @($risk.alsoCheck); files = New-Object System.Collections.ArrayList }
                [void]$risks.Add($riskSeen[$key])
            }
            [void]$riskSeen[$key].files.Add($path)
        }
    }
}

$testable = @('PLY', 'CMB', 'ABL', 'UI', 'VIT', 'ENT', 'EVT', 'STAT', 'POOL', 'MAP', 'PATH', 'ITEM', 'STATIC')
$deep = New-Object System.Collections.ArrayList
foreach ($sys in $bySystem.Keys) { if ($testable -contains $sys -and -not $deep.Contains($sys)) { [void]$deep.Add($sys) } }
foreach ($r in $risks) { foreach ($sys in $r.alsoCheck) { if ($testable -contains $sys -and -not $deep.Contains($sys)) { [void]$deep.Add($sys) } } }
$deepSorted = @($deep | Sort-Object)

# New runtime scripts and assets = candidate new features with no case yet.
$newScripts = @($fileState.GetEnumerator() | Where-Object { $_.Value -eq 'A' -and $_.Key -match '^Assets/Script/.*\.cs$' -and $_.Key -notmatch '/Editor/' } | ForEach-Object { $_.Key } | Sort-Object)
$newAssets = @($fileState.GetEnumerator() | Where-Object { $_.Value -eq 'A' -and $_.Key -match '^Assets/(SO|Prefab)/.*\.(asset|prefab)$' } | ForEach-Object { $_.Key } | Sort-Object)
$deleted = @($fileState.GetEnumerator() | Where-Object { $_.Value -eq 'D' -and $_.Key -match '^Assets/' -and $_.Key -notmatch '\.meta$' } | ForEach-Object { $_.Key } | Sort-Object)

# Classes the catalog does not mention yet.
$catalog = if (Test-Path $CatalogFile) { Get-Content -Raw -Encoding UTF8 $CatalogFile } else { '' }
$uncovered = @($newScripts | Where-Object { $catalog -notmatch [regex]::Escape([IO.Path]::GetFileNameWithoutExtension($_)) })

# --- write JSON ---------------------------------------------------------------------------------------
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
$systemsOut = [ordered]@{}
foreach ($k in ($bySystem.Keys | Sort-Object)) { $systemsOut[$k] = @($bySystem[$k]) }
$result = [ordered]@{
    base = $baseSha; head = $headSha; baseRef = $Base; headRef = $Head
    commits = @($commits); merges = $mergeCount; files = $fileState.Count
    systems = $systemsOut; deep = $deepSorted; risks = @($risks)
    newScripts = $newScripts; newAssets = $newAssets; deleted = $deleted; uncoveredNewScripts = $uncovered
}
[IO.File]::WriteAllText((Join-Path $OutDir 'change-impact.json'), ($result | ConvertTo-Json -Depth 8), $utf8)

# --- write Markdown -----------------------------------------------------------------------------------
$sb = New-Object System.Text.StringBuilder
function L([string]$s = '') { [void]$sb.AppendLine($s) }
L '# Weekly change impact'
L ''
L "- Range: ``$Base`` (``$baseSha``) .. ``$Head`` (``$headSha``)"
L "- $($commits.Count) commits (+$mergeCount merge commits), $($fileState.Count) files changed"
L "- Systems that get a deep (P2) run this week: **$(if ($deepSorted.Count) { $deepSorted -join ', ' } else { 'none' })**"
L '- The test results for these systems are in `report.md` next to this file (section "Changed this week").'
L ''
L '## 1. What landed'
L ''
L '| Commit | Date | Author | Subject | Files |'
L '|---|---|---|---|---|'
foreach ($c in $commits) { L "| ``$($c.sha)`` | $($c.date) | $($c.author) | $($c.subject -replace '\|', '\|') | $($c.files.Count) |" }
L ''
L '## 2. Systems touched'
L ''
L '| System | Files | Examples |'
L '|---|---|---|'
foreach ($k in ($bySystem.Keys | Sort-Object)) {
    $files = $bySystem[$k]
    $examples = ($files | Select-Object -First 4 | ForEach-Object { "``$_``" }) -join '<br>'
    if ($files.Count -gt 4) { $examples += "<br>... +$($files.Count - 4)" }
    L "| $k | $($files.Count) | $examples |"
}
L ''
L '## 3. Interaction risks (re-checked by the deep run)'
L ''
if ($risks.Count -eq 0) { L 'None matched.' } else {
    L '| Changed | Why it can break something else | Also checked |'
    L '|---|---|---|'
    foreach ($r in $risks) {
        $files = ($r.files | Select-Object -First 3 | ForEach-Object { "``$_``" }) -join '<br>'
        if ($r.files.Count -gt 3) { $files += "<br>... +$($r.files.Count - 3)" }
        L "| $files | $($r.why) | $($r.alsoCheck -join ', ') |"
    }
}
L ''
L '## 4. New and deleted'
L ''
L "**New runtime scripts ($($newScripts.Count))** - a script not named in ``qa-automation/cases/case-catalog.md`` is marked *no case yet*:"
L ''
if ($newScripts.Count -eq 0) { L '- none' } else { foreach ($s in $newScripts) { L "- ``$s`` - $(if ($uncovered -contains $s) { '*no case yet*' } else { 'covered' })" } }
L ''
L "**New assets ($($newAssets.Count))**:"
L ''
if ($newAssets.Count -eq 0) { L '- none' } else { foreach ($s in ($newAssets | Select-Object -First 30)) { L "- ``$s``" }; if ($newAssets.Count -gt 30) { L "- ... +$($newAssets.Count - 30)" } }
L ''
L "**Deleted ($($deleted.Count))**:"
L ''
if ($deleted.Count -eq 0) { L '- none' } else { foreach ($s in ($deleted | Select-Object -First 30)) { L "- ``$s``" } }
L ''
L '## 5. Suggested new cases'
L ''
L 'Filled in by the Friday Claude task (part C). Until then: one case per *no case yet* script above, AUTO when'
L 'the result is a number, state or event, MANUAL when it needs eyes (animation, VFX, layout, feel).'
[IO.File]::WriteAllText((Join-Path $OutDir 'weekly-change-impact.md'), $sb.ToString(), $utf8)

Write-Output ($deepSorted -join ',')
