<#
.SYNOPSIS
  Turns the raw Unity results.json into the weekly report: applies the KNOWN mapping, compares with the
  previous successful run (state/history.jsonl), flags flaky cases, and computes the run's exit code.

.OUTPUTS
  <OutDir>/report.md, <OutDir>/summary.json. Writes the exit code (0 or 1) to the pipeline.
#>
param(
    [Parameter(Mandatory = $true)][string]$OutDir,
    [Parameter(Mandatory = $true)][string]$Repo,
    [string]$RunInfoFile = '',
    [string]$KnownFile = '',
    [string]$HistoryFile = '',
    [int]$FlakyWindow = 5,
    [switch]$WriteStaticBaseline
)
$ErrorActionPreference = 'Stop'
# Defaults resolved here: $PSScriptRoot is empty inside a param block under Windows PowerShell 5.1.
$qaRoot = Split-Path -Parent $PSScriptRoot
if (-not $RunInfoFile) { $RunInfoFile = Join-Path $OutDir 'run-info.json' }
if (-not $KnownFile) { $KnownFile = Join-Path $qaRoot 'config\known-issues.json' }
if (-not $HistoryFile) { $HistoryFile = Join-Path $qaRoot 'state\history.jsonl' }
$utf8 = New-Object System.Text.UTF8Encoding $false

$results = Get-Content -Raw -Encoding UTF8 (Join-Path $OutDir 'results.json') | ConvertFrom-Json
$runInfo = if (Test-Path $RunInfoFile) { Get-Content -Raw -Encoding UTF8 $RunInfoFile | ConvertFrom-Json } else { $null }
$impactFile = Join-Path $OutDir 'change-impact.json'
$impact = if (Test-Path $impactFile) { Get-Content -Raw -Encoding UTF8 $impactFile | ConvertFrom-Json } else { $null }
$known = (Get-Content -Raw -Encoding UTF8 $KnownFile | ConvertFrom-Json).issues
$bugsDir = Join-Path $Repo 'production\qa\bugs'

# --- bug file status ----------------------------------------------------------------------------------
$bugCache = @{}
function BugState([string]$bug) {
    if ($bugCache.ContainsKey($bug)) { return $bugCache[$bug] }
    $file = Join-Path $bugsDir "$bug.md"
    $state = 'missing'
    if (Test-Path $file) {
        $status = (Get-Content -Encoding UTF8 $file | Where-Object { $_ -match '^\*\*Status' } | Select-Object -First 1)
        if ($status -match '(?i)\b(fixed|closed|resolved)\b' -and $status -notmatch '(?i)\b(partial|open)\b') { $state = 'fixed' } else { $state = 'open' }
    }
    $bugCache[$bug] = $state
    return $state
}

# Static findings that predate the automation (legacy assets). They are debt, reported as a count, and do
# not fail the run; a finding not in this list does. Rewrite with -WriteStaticBaseline after a triage.
$baselineFile = Join-Path $qaRoot 'config\static-baseline.txt'
$baseline = New-Object 'System.Collections.Generic.HashSet[string]'
if (Test-Path $baselineFile) { Get-Content -Encoding UTF8 $baselineFile | Where-Object { $_ -and -not $_.StartsWith('#') } | ForEach-Object { [void]$baseline.Add($_) } }
$newStaticFindings = New-Object System.Collections.ArrayList

function FindingLines($case) {
    @($case.notes | Where-Object { $_ -notmatch '^ok:' -and $_ -notmatch '^FAIL: \d+ finding' -and $_ -notmatch '^build scenes:' -and $_ -notmatch '^\.\.\. ' -and $_ -notmatch '^(OBSERVED|SKIP):' })
}

# --- effective verdicts -------------------------------------------------------------------------------
$rows = New-Object System.Collections.ArrayList
foreach ($c in $results.cases) {
    $verdict = $c.verdict
    $bugs = @()
    $remark = ''
    if ($verdict -eq 'FAIL') {
        $entries = @($known | Where-Object { $c.id -match $_.pattern })
        $valid = @($entries | Where-Object { (BugState $_.bug) -eq 'open' })
        $fixedOnes = @($entries | Where-Object { (BugState $_.bug) -eq 'fixed' })
        $missing = @($entries | Where-Object { (BugState $_.bug) -eq 'missing' })
        if ($fixedOnes.Count) { $remark = "mapped to $(($fixedOnes | ForEach-Object bug) -join ', ') whose file says fixed - regression?" }
        if ($missing.Count) { $remark = "known-issues.json names $(($missing | ForEach-Object bug) -join ', ') but no bug file exists - entry ignored" }
        $plain = @($valid | Where-Object { -not $_.findings })
        $byFinding = @($valid | Where-Object { $_.findings })
        if ($plain.Count) {
            $verdict = 'KNOWN'; $bugs = @($plain | ForEach-Object bug)
        }
        elseif ($c.system -eq 'STATIC' -or $byFinding.Count) {
            $all = FindingLines $c
            $debt = @($all | Where-Object { $baseline.Contains($_) })
            $lines = @($all | Where-Object { -not $baseline.Contains($_) })
            $unmatched = @($lines | Where-Object { $l = $_; -not ($byFinding | Where-Object { $l -match $_.findings }) })
            if ($all.Count -gt 0 -and $unmatched.Count -eq 0) {
                $verdict = 'KNOWN'
                $bugs = @($byFinding | Where-Object { $f = $_.findings; $lines | Where-Object { $_ -match $f } } | ForEach-Object bug | Select-Object -Unique)
                if ($debt.Count) { $bugs += "baseline debt ($($debt.Count))" }
            }
            elseif ($all.Count -gt 0) {
                $remark = "$($unmatched.Count) new finding(s) not covered by a known bug or the baseline ($($debt.Count) baseline)"
                [void]$newStaticFindings.AddRange([string[]]$unmatched)
            }
        }
    }
    [void]$rows.Add([pscustomobject]@{ id = $c.id; title = $c.title; system = $c.system; priority = $c.priority; group = $c.group; raw = $c.verdict; verdict = $verdict; bugs = ($bugs -join ', '); remark = $remark; notes = @($c.notes) })
}

if ($WriteStaticBaseline) {
    $lines = @('# Static findings accepted as legacy debt. One finding per line, exact text from QA-STATIC-* notes.') +
        @($baseline) + @($newStaticFindings) | Select-Object -Unique
    [IO.File]::WriteAllText($baselineFile, (($lines -join "`n") + "`n"), $utf8)
    Write-Host "static baseline written: $($lines.Count - 1) findings -> $baselineFile"
    exit 0
}

# --- history / diff / flaky ---------------------------------------------------------------------------
$history = @()
if (Test-Path $HistoryFile) { $history = @(Get-Content -Encoding UTF8 $HistoryFile | Where-Object { $_.Trim() } | ForEach-Object { $_ | ConvertFrom-Json }) }
$prev = if ($history.Count) { $history[$history.Count - 1] } else { $null }
function PrevVerdict([string]$id) {
    if (-not $prev) { return $null }
    $p = $prev.verdicts.PSObject.Properties[$id]
    if ($p) { return $p.Value } else { return $null }
}
$bad = @('FAIL', 'KNOWN')
$newFail = @(); $fixed = @(); $stillFail = @(); $newCases = @(); $flaky = @()
foreach ($r in $rows) {
    $p = PrevVerdict $r.id
    if ($null -eq $p) { $newCases += $r }
    if ($r.verdict -eq 'FAIL' -and $p -ne 'FAIL') { $newFail += $r }
    elseif ($r.verdict -eq 'FAIL') { $stillFail += $r }
    if ($r.verdict -eq 'PASS' -and $bad -contains $p) { $fixed += $r }

    $seq = @($history | Select-Object -Last ($FlakyWindow - 1) | ForEach-Object { $q = $_.verdicts.PSObject.Properties[$r.id]; if ($q) { $q.Value } }) + @($r.verdict)
    $seq = @($seq | Where-Object { $_ -in @('PASS', 'FAIL', 'KNOWN') } | ForEach-Object { if ($_ -eq 'PASS') { 'P' } else { 'F' } })
    $flips = 0
    for ($i = 1; $i -lt $seq.Count; $i++) { if ($seq[$i] -ne $seq[$i - 1]) { $flips++ } }
    if ($flips -ge 2) { $flaky += [pscustomobject]@{ row = $r; seq = ($seq -join '') } }
}
$notRun = @()
if ($prev) {
    $ids = @{}; foreach ($r in $rows) { $ids[$r.id] = $true }
    $notRun = @($prev.verdicts.PSObject.Properties | Where-Object { -not $ids.ContainsKey($_.Name) } | ForEach-Object { $_.Name })
}

function Count([string]$v) { @($rows | Where-Object { $_.verdict -eq $v }).Count }
$unknownFails = Count 'FAIL'
$exitCode = if ($unknownFails -gt 0) { 1 } else { 0 }

# --- summary.json ---------------------------------------------------------------------------------------
$verdicts = [ordered]@{}
foreach ($r in $rows) { $verdicts[$r.id] = $r.verdict }
$summary = [ordered]@{
    run = if ($runInfo) { $runInfo.run } else { (Get-Date -Format 'yyyyMMdd-HHmm') }
    sprint = if ($runInfo) { $runInfo.sprint } else { '' }
    head = if ($runInfo) { $runInfo.head } else { '' }
    exitCode = $exitCode
    totals = [ordered]@{ total = $rows.Count; PASS = (Count 'PASS'); FAIL = $unknownFails; KNOWN = (Count 'KNOWN'); SKIP = (Count 'SKIP'); OBSERVED = (Count 'OBSERVED') }
    newFail = @($newFail | ForEach-Object id); fixed = @($fixed | ForEach-Object id); flaky = @($flaky | ForEach-Object { $_.row.id })
    verdicts = $verdicts
}
[IO.File]::WriteAllText((Join-Path $OutDir 'summary.json'), ($summary | ConvertTo-Json -Depth 6), $utf8)

# --- report.md ------------------------------------------------------------------------------------------
$sb = New-Object System.Text.StringBuilder
function L([string]$s = '') { [void]$sb.AppendLine($s) }
function Esc([string]$s) { ($s -replace '\|', '\|') -replace "`r?`n", ' ' }
function Details($r) {
    $n = @($r.notes | Where-Object { $_ -notmatch '^ok:' } | Select-Object -First 6)
    $t = ($n | ForEach-Object { Esc $_ }) -join '<br>'
    if ($r.remark) { $t = "_$(Esc $r.remark)_<br>$t" }
    return $t
}
function Table($list, [string]$empty = 'none') {
    if (-not $list -or @($list).Count -eq 0) { L "- $empty"; return }
    L '| ID | Pri | Case | Result | Bug | Details |'
    L '|---|---|---|---|---|---|'
    foreach ($r in $list) { L "| $($r.id) | $($r.priority) | $(Esc $r.title) | **$($r.verdict)** | $($r.bugs) | $(Details $r) |" }
}

L '# Weekly QA automation report'
L ''
if ($runInfo) {
    L "- Run ``$($runInfo.run)`` | sprint ``$($runInfo.sprint)`` at ``$($runInfo.head)`` | baseline ``$($runInfo.base)``"
    L "- Owner branch ``$($runInfo.ownerBranch)``$(if ($runInfo.autoCommit) { " | auto-commit ``$($runInfo.autoCommit)`` (coding need check)" } else { ' | tree was clean' })"
}
L "- Unity $($results.unity) | priority <= $($results.maxPriority) + deep P2 for: $(if ($results.deep) { $results.deep } else { 'none' }) | seed $($results.seed)"
L "- **Exit $exitCode** - $(if ($exitCode -eq 0) { 'no unknown FAIL' } else { "$unknownFails unknown FAIL(s)" })"
L ''
L '| PASS | FAIL | KNOWN | SKIP | OBSERVED | Total |'
L '|---|---|---|---|---|---|'
L "| $(Count 'PASS') | $unknownFails | $(Count 'KNOWN') | $(Count 'SKIP') | $(Count 'OBSERVED') | $($rows.Count) |"
L ''
L 'FAIL = unexpected, needs a look. KNOWN = fails because of an open bug file (listed). OBSERVED = recorded where the design is undecided. SKIP = not testable as built (reason in Details).'
L ''
L '## 1. Compared with the previous run'
L ''
if (-not $prev) { L '_No previous run recorded: this is the baseline._'; L '' }
else { L "Previous: run ``$($prev.run)`` at ``$($prev.head)``."; L '' }
L "### New FAIL ($($newFail.Count))"
L ''
Table $newFail
L ''
L "### Fixed since last run ($($fixed.Count))"
L ''
Table $fixed
L ''
L "### Still failing ($($stillFail.Count))"
L ''
Table $stillFail
L ''
L "### Flaky suspects ($($flaky.Count))"
L ''
if ($flaky.Count -eq 0) { L '- none' } else { foreach ($f in $flaky) { L "- $($f.row.id): last runs ``$($f.seq)`` (P = pass, F = fail/known) - see README › Flaky cases" } }
L ''
if ($notRun.Count) { L "Not run this week (P2 of an unchanged system, or removed): $($notRun.Count) case(s)."; L '' }
L '## 2. Changed this week'
L ''
if ($impact) {
    L "Change range ``$($impact.base)``..``$($impact.head)``: $(@($impact.commits).Count) commits, $($impact.files) files. Details: ``weekly-change-impact.md``."
    L ''
    $deepSet = @($impact.deep)
    $touched = @($rows | Where-Object { $deepSet -contains $_.system })
    L "Cases of the touched systems ($($deepSet -join ', ')): $(@($touched | Where-Object verdict -eq 'PASS').Count) PASS, $(@($touched | Where-Object verdict -eq 'FAIL').Count) FAIL, $(@($touched | Where-Object verdict -eq 'KNOWN').Count) KNOWN."
    L ''
    Table @($touched | Where-Object { $_.verdict -ne 'PASS' }) 'all PASS'
    $uncovered = @($impact.uncoveredNewScripts)
    L ''
    $uncoveredText = if ($uncovered.Count) { ($uncovered | ForEach-Object { '`' + $_ + '`' }) -join ', ' } else { 'none' }
    L "New scripts with no case yet: $uncoveredText"
}
else { L '_No change-impact scan for this run._' }
L ''
L '## 3. Hygiene'
L ''
if ($runInfo -and $runInfo.reverted -and @($runInfo.reverted).Count) {
    L 'Files the run modified and the runner reverted (BUG-063 lesson):'
    foreach ($f in $runInfo.reverted) { L "- ``$f``" }
}
else { L '- No tracked asset was left modified by the run.' }
if ($runInfo -and $runInfo.untracked -and @($runInfo.untracked).Count) {
    L ''
    L 'Untracked files that appeared during the run (left in place):'
    foreach ($f in $runInfo.untracked) { L "- ``$f``" }
}
L ''
L '## 4. All results'
L ''
foreach ($g in ($rows | Group-Object group)) {
    L "### $($g.Name)"
    L ''
    Table $g.Group
    L ''
}
L 'Manual cases (visual, feel, layout): `qa-automation/manual/manual-weekly-checklist.md`.'
[IO.File]::WriteAllText((Join-Path $OutDir 'report.md'), $sb.ToString(), $utf8)

# --- history ----------------------------------------------------------------------------------------------
$entry = [ordered]@{ run = $summary.run; sprint = $summary.sprint; head = $summary.head; exitCode = $exitCode; verdicts = $verdicts }
New-Item -ItemType Directory -Force -Path (Split-Path $HistoryFile) | Out-Null
[IO.File]::AppendAllText($HistoryFile, (($entry | ConvertTo-Json -Depth 4 -Compress) + "`n"), $utf8)

Write-Output $exitCode
