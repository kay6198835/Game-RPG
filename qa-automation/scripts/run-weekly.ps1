<#
.SYNOPSIS
  Weekly QA automation run (Friday 23:00). One entry point for the scheduler and for a manual run.

.DESCRIPTION
  1. Refuses to run while a Unity Editor is open (exit 3).
  2. Commits any uncommitted work on the owner's current branch as "coding need check" (local, never pushed).
  3. Fetches, finds the current sprint branch (highest origin/sprint-NN), checks out qa/weekly-automation
     and merges origin/sprint-NN into it (local).
  4. Scans the changes since the last successful run (scan-changes.ps1) -> systems that get a deep P2 run.
  5. Compile check in batchmode (exit 4 on error CS).
  6. Runs the Unity QA runner (STATIC + 4 Play Mode groups), P0 + P1 + P2 of the touched systems.
  7. Reverts any tracked file the run modified outside the QA folders (BUG-063 lesson), writes the report
     (diff-results.ps1), commits reports + state on qa/weekly-automation (local), and switches back.

  Exit codes: 0 no unknown FAIL | 1 unknown FAIL | 2 runner/git error | 3 skipped (Editor open) | 4 compile error.
  Every run leaves a log in %LOCALAPPDATA%\GameRPG-QA\logs and, when the branch could be reached, in
  qa-automation/reports/<run>/run.log on qa/weekly-automation (pushed, then merged into the sprint branch). The latest status is in
  qa-automation/state/latest.json on that branch (git show qa/weekly-automation:qa-automation/state/latest.json).

.EXAMPLE
  # Scheduler / manual, from anywhere (the script may not exist on the owner's branch):
  git -C D:\Fork\Game-RPG show qa/weekly-automation:qa-automation/scripts/run-weekly.ps1 > $env:TEMP\qa-run-weekly.ps1
  powershell -NoProfile -ExecutionPolicy Bypass -File $env:TEMP\qa-run-weekly.ps1
#>
param(
    [string]$Project = 'D:\Fork\Game-RPG',
    [string]$QaBranch = 'qa/weekly-automation',
    [string]$Since = '',
    [string]$MaxPriority = 'P1',
    [string]$Deep = '',
    [int]$CompileTimeoutMin = 40,
    [int]$UnityTimeoutMin = 90,
    # PM worktree used to merge the QA branch into the sprint branch without touching the owner's folder.
    [string]$PmWorktree = 'D:\Fork\Game-RPG-pm',
    [switch]$NoSprintMerge
)
$ErrorActionPreference = 'Stop'
$utf8 = New-Object System.Text.UTF8Encoding $false
$run = Get-Date -Format 'yyyyMMdd-HHmm'
$localRoot = Join-Path $env:LOCALAPPDATA 'GameRPG-QA'
$logDir = Join-Path $localRoot 'logs'
New-Item -ItemType Directory -Force -Path $logDir | Out-Null
$log = Join-Path $logDir "run-$run.log"
$lock = Join-Path $localRoot 'run.lock'

function Log([string]$msg) {
    $line = "[$(Get-Date -Format 'HH:mm:ss')] $msg"
    Write-Host $line
    [IO.File]::AppendAllText($log, $line + "`r`n", $utf8)
}

function Git {
    $GitArgs = @($args)
    $prev = $ErrorActionPreference; $ErrorActionPreference = 'Continue'
    $out = & git.exe -C $Project @GitArgs 2>&1 | ForEach-Object { "$_" }
    $code = $LASTEXITCODE
    $ErrorActionPreference = $prev
    if ($code -ne 0) { throw "git $($GitArgs -join ' ') (exit $code): $($out -join ' | ')" }
    return $out
}

function PmGit {
    $GitArgs = @($args)
    $prev = $ErrorActionPreference; $ErrorActionPreference = 'Continue'
    $out = & git.exe -C $PmWorktree @GitArgs 2>&1 | ForEach-Object { "$_" }
    $code = $LASTEXITCODE
    $ErrorActionPreference = $prev
    if ($code -ne 0) { throw "git(pm) $($GitArgs -join ' ') (exit $code): $($out -join ' | ')" }
    return $out
}

# Merges the QA branch into the sprint branch in the PM worktree and pushes it (owner decision 2026-11-01),
# so every new sprint carries the QA code. Only after a run whose tests ran (the compile check passed).
# Never forced; a conflict or a dirty PM worktree leaves the sprint branch untouched and is logged.
function Merge-IntoSprint([string]$sprintRef) {
    $sprintName = $sprintRef -replace '^origin/', ''
    if (-not (Test-Path $PmWorktree)) { throw "PM worktree $PmWorktree not found" }
    if (@(PmGit status --porcelain | Where-Object { $_ }).Count -gt 0) { throw "PM worktree $PmWorktree is not clean" }
    for ($attempt = 1; $attempt -le 2; $attempt++) {
        PmGit fetch origin --prune | Out-Null
        PmGit checkout --detach $sprintRef | Out-Null
        $behind = @(PmGit rev-list "$sprintRef..$QaBranch" | Where-Object { $_ }).Count
        if ($behind -eq 0) { Log "$sprintName already contains $QaBranch"; return }
        try { PmGit merge --no-ff $QaBranch -m "Merge $QaBranch into $sprintName (QA run $run)" | Out-Null }
        catch {
            try { PmGit merge --abort | Out-Null } catch { }
            PmGit checkout --detach $sprintRef | Out-Null
            throw "merge of $QaBranch into $sprintName conflicted: $($_.Exception.Message)"
        }
        try {
            PmGit push origin "HEAD:refs/heads/$sprintName" | Out-Null
            Log "merged $QaBranch into $sprintName and pushed ($((PmGit rev-parse --short HEAD | Select-Object -Last 1).Trim()))"
            return
        }
        catch {
            Log "push to $sprintName rejected (attempt $attempt): $($_.Exception.Message)"
            PmGit checkout --detach $sprintRef | Out-Null
        }
    }
    throw "push to $sprintName rejected twice"
}

function TryGit {
    $GitArgs = @($args)
    try { return @(Git @GitArgs) } catch { Log "  (ignored) $($_.Exception.Message)"; return $null }
}

$status = [ordered]@{
    run = $run; status = 'STARTED'; exitCode = 2; sprint = ''; head = ''; base = ''; ownerBranch = ''; autoCommit = ''
    reportDir = ''; totals = $null; message = ''; log = $log; finished = ''
}

function Save-LocalStatus { [IO.File]::WriteAllText((Join-Path $localRoot 'latest.json'), ($status | ConvertTo-Json -Depth 5), $utf8) }

# Commits one file onto a branch without checking it out (plumbing), for statuses written before checkout.
function Commit-FileToBranch([string]$branch, [string]$repoPath, [string]$content, [string]$message) {
    $tmp = [IO.Path]::GetTempFileName()
    $idx = [IO.Path]::GetTempFileName()
    try {
        [IO.File]::WriteAllText($tmp, $content, $utf8)
        Remove-Item $idx
        $env:GIT_INDEX_FILE = $idx
        Git read-tree $branch | Out-Null
        $blob = (Git hash-object -w $tmp | Select-Object -Last 1).Trim()
        Git update-index --add --cacheinfo "100644,$blob,$repoPath" | Out-Null
        $tree = (Git write-tree | Select-Object -Last 1).Trim()
        Remove-Item Env:\GIT_INDEX_FILE
        $commit = (Git commit-tree $tree -p $branch -m $message | Select-Object -Last 1).Trim()
        Git update-ref "refs/heads/$branch" $commit | Out-Null
        Log "status committed to $branch ($commit)"
    }
    catch { Log "could not commit status to ${branch}: $($_.Exception.Message)" }
    finally {
        if (Test-Path Env:\GIT_INDEX_FILE) { Remove-Item Env:\GIT_INDEX_FILE }
        Remove-Item $tmp, $idx -ErrorAction SilentlyContinue
    }
}

function Unity-Exe {
    $line = Get-Content (Join-Path $Project 'ProjectSettings\ProjectVersion.txt') | Where-Object { $_ -match '^m_EditorVersion:' } | Select-Object -First 1
    $version = ($line -split ':\s*')[1].Trim()
    $exe = "C:\Program Files\Unity\Hub\Editor\$version\Editor\Unity.exe"
    if (-not (Test-Path $exe)) { throw "Unity $version not found at $exe" }
    return $exe
}

function Run-Unity([string]$exe, [string]$arguments, [int]$timeoutMin) {
    Log "unity $arguments"
    $p = Start-Process -FilePath $exe -ArgumentList $arguments -PassThru -WindowStyle Minimized
    if (-not $p.WaitForExit($timeoutMin * 60 * 1000)) {
        Log "TIMEOUT after $timeoutMin min - killing Unity"
        try { $p.Kill() } catch { }
        Start-Sleep -Seconds 5
        Get-Process Unity -ErrorAction SilentlyContinue | Where-Object { $_.StartTime -ge $p.StartTime } | Stop-Process -Force -ErrorAction SilentlyContinue
        return -1
    }
    return $p.ExitCode
}

$ownerRef = $null
$switched = $false
$exitCode = 2
try {
    Log "QA weekly run $run, project $Project"
    if (Test-Path $lock) {
        $age = (Get-Date) - (Get-Item $lock).LastWriteTime
        if ($age.TotalHours -lt 4) { throw "another run holds $lock (age $([int]$age.TotalMinutes) min)" }
        Log "stale lock ($([int]$age.TotalHours) h) removed"
    }
    Set-Content -Path $lock -Value $PID

    # 1. Editor open?
    $editors = @(Get-Process -Name Unity -ErrorAction SilentlyContinue)
    if ($editors.Count -gt 0) {
        $exitCode = 3
        $status.status = 'SKIPPED'; $status.exitCode = 3
        $status.message = "Unity Editor is open (pid $($editors.Id -join ', ')); the run did not touch git or the project. Close the Editor and run qa-automation/scripts/run-weekly.ps1 by hand."
        Log $status.message
        throw [System.OperationCanceledException]::new($status.status)
    }

    # 2. Owner branch + auto-commit
    $ownerBranch = (Git rev-parse --abbrev-ref HEAD | Select-Object -Last 1).Trim()
    $ownerRef = if ($ownerBranch -eq 'HEAD') { (Git rev-parse HEAD | Select-Object -Last 1).Trim() } else { $ownerBranch }
    $status.ownerBranch = $ownerBranch
    $state = Join-Path $Project '.git'
    if ((Test-Path (Join-Path $state 'MERGE_HEAD')) -or (Test-Path (Join-Path $state 'rebase-merge')) -or (Test-Path (Join-Path $state 'rebase-apply'))) {
        throw 'a merge or rebase is in progress in the owner folder - not touching it'
    }
    # QA build output (scenes) or a QA folder left untracked by an interrupted run is never the owner's
    # work: exclude it from the auto-commit.
    $exclude = @(':(exclude)Assets/QA', ':(exclude)Assets/QA.meta', ':(exclude)qa-automation')
    $dirty = @(Git status --porcelain '--' . @exclude | Where-Object { $_ })
    if ($dirty.Count -gt 0) {
        Log "owner tree dirty ($($dirty.Count) paths) on $ownerBranch -> commit 'coding need check'"
        Git add -A '--' . @exclude | Out-Null
        Git commit -m 'coding need check' | Out-Null
        $status.autoCommit = (Git rev-parse --short HEAD | Select-Object -Last 1).Trim()
        Log "auto-commit $($status.autoCommit) on $ownerBranch (local, not pushed)"
    }

    # 3. Sprint branch, QA branch, merge
    Git fetch origin --prune | Out-Null
    $sprints = Git for-each-ref --format='%(refname:short)' 'refs/remotes/origin/sprint-*' |
        ForEach-Object { $_.Trim("'") } | Where-Object { $_ -match '^origin/sprint-(\d+)$' } |
        Sort-Object { [int]($_ -replace '^origin/sprint-', '') }
    $sprint = @($sprints)[-1]
    if (-not $sprint) { throw 'no origin/sprint-NN branch found' }
    $status.sprint = $sprint
    $status.head = (Git rev-parse --short=10 $sprint | Select-Object -Last 1).Trim()
    Log "sprint branch $sprint at $($status.head)"

    if (-not (TryGit rev-parse --verify $QaBranch)) { throw "local branch $QaBranch does not exist" }
    if ($ownerBranch -ne $QaBranch) {
        Git checkout $QaBranch | Out-Null
        $switched = $true
    }
    $mergeOut = $null
    try { $mergeOut = Git merge --no-edit $sprint }
    catch {
        TryGit merge --abort | Out-Null
        throw "merge of $sprint into $QaBranch failed (conflict?): $($_.Exception.Message)"
    }
    Log "merged $sprint into $QaBranch"

    # 4. Change scan
    $out = Join-Path $Project "qa-automation\reports\$run"
    New-Item -ItemType Directory -Force -Path $out | Out-Null
    $status.reportDir = "qa-automation/reports/$run"
    $lastFile = Join-Path $Project 'qa-automation\state\last-success.json'
    $base = $Since
    if (-not $base -and (Test-Path $lastFile)) { $base = (Get-Content -Raw $lastFile | ConvertFrom-Json).sprintHead }
    if (-not $base) {
        $n = [int]($sprint -replace '^origin/sprint-', '')
        $previous = "origin/sprint-{0:D2}" -f ($n - 1)
        if (TryGit rev-parse --verify $previous) { $base = $previous } else { $base = "$sprint~20" }
        Log "no previous successful run: baseline = $base"
    }
    $status.base = $base
    $deepFound = & (Join-Path $Project 'qa-automation\scripts\scan-changes.ps1') -Repo $Project -Base $base -Head $sprint -OutDir $out
    $deepList = if ($Deep) { $Deep } else { "$deepFound" }
    Log "change scan $base..$sprint -> deep P2 for: $(if ($deepList) { $deepList } else { 'none' })"

    # 5. Compile check
    $unity = Unity-Exe
    $compileLog = Join-Path $out 'unity-compile.log'
    $code = Run-Unity $unity "-batchmode -nographics -quit -projectPath `"$Project`" -logFile `"$compileLog`"" $CompileTimeoutMin
    $errors = @()
    if (Test-Path $compileLog) { $errors = @(Select-String -Path $compileLog -Pattern 'error CS\d{4}' | ForEach-Object { $_.Line.Trim() } | Select-Object -Unique) }
    if ($errors.Count -gt 0) {
        $exitCode = 4
        $status.status = 'COMPILE_ERROR'; $status.exitCode = 4
        $status.message = "$($errors.Count) compile error(s); first: $($errors[0])"
        Log $status.message
        [IO.File]::WriteAllText((Join-Path $out 'report.md'), "# Weekly QA automation report`n`n**Exit 4 - compile error.** No test ran.`n`n" + (($errors | Select-Object -First 20 | ForEach-Object { "- ``$_``" }) -join "`n") + "`n", $utf8)
        throw [System.OperationCanceledException]::new($status.status)
    }
    if ($code -ne 0) { Log "compile pass exit $code with no error CS line (see unity-compile.log) - continuing" }

    # 5b. EditMode unit tests (Assets/Tests/EditMode). Each NUnit test becomes one case, system UNIT, so the
    # diff, KNOWN mapping and flaky detection treat it like any other case. A branch with no EditMode tests
    # (older sprint) is skipped, not failed.
    $unityCases = @()
    $unitDir = Join-Path $Project 'Assets\Tests\EditMode'
    if (Test-Path $unitDir) {
        $unitXml = Join-Path $out 'unit-results.xml'
        $unitLog = Join-Path $out 'unity-unit.log'
        $code = Run-Unity $unity "-batchmode -nographics -projectPath `"$Project`" -runTests -testPlatform EditMode -testResults `"$unitXml`" -logFile `"$unitLog`"" $CompileTimeoutMin
        Log "unity EditMode tests exit $code"
        if (Test-Path $unitXml) {
            [xml]$nunit = Get-Content -Raw -Encoding UTF8 $unitXml
            foreach ($t in @($nunit.SelectNodes('//test-case'))) {
                $res = $t.GetAttribute('result'); $cls = $t.GetAttribute('classname') -replace '^.*\.', ''; $nm = $t.GetAttribute('name')
                $verdict = switch ($res) { 'Passed' { 'PASS' } 'Failed' { 'FAIL' } default { 'SKIP' } }
                $notes = @()
                if ($t.failure -and $t.failure.message) { $notes += "FAIL: $($t.failure.message.InnerText.Trim())" }
                elseif ($verdict -eq 'PASS') { $notes += 'ok: passed' }
                elseif ($t.reason -and $t.reason.message) { $notes += "SKIP: $($t.reason.message.InnerText.Trim())" }
                else { $notes += "SKIP: result $res" }
                $unityCases += [pscustomobject][ordered]@{
                    id = "QA-UNIT.$cls.$nm"; title = "EditMode: $($t.GetAttribute('fullname'))"
                    system = 'UNIT'; priority = 'P0'; group = 'UNIT'; verdict = $verdict
                    seconds = [double]$t.GetAttribute('duration'); notes = @($notes)
                }
            }
            Log "EditMode tests: $($unityCases.Count) case(s)"
        }
        else {
            $unityCases += [pscustomobject][ordered]@{
                id = 'QA-UNIT-RUNNER'; title = 'EditMode test run wrote its results'; system = 'UNIT'; priority = 'P0'; group = 'UNIT'
                verdict = 'FAIL'; seconds = 0.0; notes = @("FAIL: no unit-results.xml (Unity exit $code); read unity-unit.log in %LOCALAPPDATA%\GameRPG-QA\logs")
            }
            Log 'EditMode tests produced no results xml'
        }
    }
    else { Log 'no Assets/Tests/EditMode on this branch - unit tests skipped' }

    # 6. Test run
    $runLog = Join-Path $out 'unity-run.log'
    $qaArgs = "-projectPath `"$Project`" -executeMethod QA.Automation.Editor.QARunner.RunFromCommandLine -qaOut `"$out`" -qaMax $MaxPriority -logFile `"$runLog`""
    if ($deepList) { $qaArgs += " -qaDeep $deepList" }
    $code = Run-Unity $unity $qaArgs $UnityTimeoutMin
    Log "unity test run exit $code"
    if (-not (Test-Path (Join-Path $out 'results.json'))) {
        # A crash of the Editor itself (GPU device reset, out of memory) is not a test result: retry once
        # and keep the first log, so the report still shows that it happened.
        $crash = if (Test-Path $runLog) { @(Select-String -Path $runLog -Pattern 'device reset/removed|unrecoverable error|Crash!!!|Out of memory' | Select-Object -First 1 | ForEach-Object { $_.Line.Trim() }) } else { @() }
        Log "no results.json; crash line: $(if ($crash.Count) { $crash[0] } else { 'none found' }) - retrying once"
        Get-Process Unity -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
        if (Test-Path $runLog) { Move-Item $runLog (Join-Path $out 'unity-run-attempt1.log') -Force }
        $code = Run-Unity $unity $qaArgs $UnityTimeoutMin
        Log "unity test run (retry) exit $code"
        if (-not (Test-Path (Join-Path $out 'results.json'))) { throw "Unity produced no results.json after a retry (exit $code) - read $runLog" }
    }

    # 6b. Merge the EditMode cases into results.json so diff-results.ps1 sees one list.
    if ($unityCases.Count -gt 0) {
        $resultsFile = Join-Path $out 'results.json'
        $res = Get-Content -Raw -Encoding UTF8 $resultsFile | ConvertFrom-Json
        $res.cases = @($res.cases) + @($unityCases)
        if (@($res.groups) -notcontains 'UNIT') { $res.groups = @($res.groups) + 'UNIT' }
        [IO.File]::WriteAllText($resultsFile, ($res | ConvertTo-Json -Depth 8), $utf8)
    }

    # 7. Hygiene: nothing outside the QA folders may stay modified
    $reverted = New-Object System.Collections.ArrayList
    $untracked = New-Object System.Collections.ArrayList
    foreach ($line in @(Git status --porcelain | Where-Object { $_ })) {
        $path = $line.Substring(3).Trim('"')
        if ($path -match '^(qa-automation/|Assets/QA/)') { continue }
        if ($line.StartsWith('??')) { [void]$untracked.Add($path); continue }
        Git checkout '--' $path | Out-Null
        [void]$reverted.Add($path)
    }
    if ($reverted.Count) { Log "reverted $($reverted.Count) file(s) modified by the run: $($reverted -join ', ')" }

    $runInfo = [ordered]@{
        run = $run; sprint = $sprint; head = $status.head; base = $base; ownerBranch = $ownerBranch; autoCommit = $status.autoCommit
        unityExit = $code; reverted = @($reverted); untracked = @($untracked)
    }
    [IO.File]::WriteAllText((Join-Path $out 'run-info.json'), ($runInfo | ConvertTo-Json -Depth 4), $utf8)
    $exitCode = [int](& (Join-Path $Project 'qa-automation\scripts\diff-results.ps1') -OutDir $out -Repo $Project | Select-Object -Last 1)
    $summary = Get-Content -Raw (Join-Path $out 'summary.json') | ConvertFrom-Json
    $status.totals = $summary.totals
    $status.status = if ($exitCode -eq 0) { 'PASS' } else { 'FAIL' }
    $status.exitCode = $exitCode
    $status.message = "PASS $($summary.totals.PASS) / FAIL $($summary.totals.FAIL) / KNOWN $($summary.totals.KNOWN) / SKIP $($summary.totals.SKIP) / OBSERVED $($summary.totals.OBSERVED); new FAIL: $(@($summary.newFail).Count)"
    Log $status.message

    [IO.File]::WriteAllText($lastFile, ([ordered]@{ run = $run; sprint = $sprint; sprintHead = $status.head; exitCode = $exitCode } | ConvertTo-Json), $utf8)
}
catch [System.OperationCanceledException] {
    Log "stopped: $($_.Exception.Message)"
}
catch {
    $exitCode = 2
    $status.status = 'RUNNER_ERROR'; $status.exitCode = 2
    $status.message = $_.Exception.Message
    Log "RUNNER ERROR: $($_.Exception.Message)"
    Log $_.ScriptStackTrace
}
finally {
    $status.finished = (Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
    Save-LocalStatus
    $statusJson = $status | ConvertTo-Json -Depth 5
    try {
        $onQa = $ownerRef -and ((Git rev-parse --abbrev-ref HEAD | Select-Object -Last 1).Trim() -eq $QaBranch)
        if ($onQa -and $status.reportDir) {
            $out = Join-Path $Project ($status.reportDir -replace '/', '\')
            New-Item -ItemType Directory -Force -Path $out | Out-Null
            Copy-Item $log (Join-Path $out 'run.log') -Force
            [IO.File]::WriteAllText((Join-Path $Project 'qa-automation\state\latest.json'), $statusJson, $utf8)
            # Unity logs are large and machine-specific; keep them local only.
            Get-ChildItem $out -Filter 'unity-*.log' | ForEach-Object { Move-Item $_.FullName (Join-Path $logDir "$run-$($_.Name)") -Force }
            Git add '--' 'qa-automation/reports' 'qa-automation/state' | Out-Null
            Git commit -m "test(qa): weekly automation run $run on $($status.sprint) - $($status.status)" | Out-Null
            Log "results committed on $QaBranch"
        }
        elseif (TryGit rev-parse --verify $QaBranch) {
            Commit-FileToBranch $QaBranch 'qa-automation/state/latest.json' $statusJson "test(qa): weekly automation run $run - $($status.status)"
        }
    }
    catch { Log "could not commit results: $($_.Exception.Message)" }
    # Off-machine backup of the QA code and reports (owner decision 2026-11-01). Never forced: a rejected
    # push is logged and the run's exit code is unchanged.
    try { Git push origin "refs/heads/${QaBranch}:refs/heads/$QaBranch" | Out-Null; Log "pushed $QaBranch to origin" }
    catch { Log "PUSH FAILED for ${QaBranch}: $($_.Exception.Message)"; $status.message += " | push of $QaBranch failed (see log)"; Save-LocalStatus }
    if (-not $NoSprintMerge -and $status.sprint -and $status.status -in @('PASS', 'FAIL')) {
        try { Merge-IntoSprint $status.sprint }
        catch { Log "SPRINT MERGE FAILED: $($_.Exception.Message)"; $status.message += " | merge into $($status.sprint) failed (see log)"; Save-LocalStatus }
    }
    elseif (-not $NoSprintMerge) { Log "sprint merge skipped: tests did not run (status $($status.status))" }
    if ($switched -and $ownerRef) {
        # The QA scenes are build output (git-ignored on the QA branch only). Left behind they would sit
        # untracked on the owner's branch, where their scripts do not exist.
        Remove-Item -Recurse -Force (Join-Path $Project 'Assets\QA\Scenes'), (Join-Path $Project 'Assets\QA\Scenes.meta') -ErrorAction SilentlyContinue
        try { Git checkout $ownerRef | Out-Null; Log "back on $ownerRef" }
        catch { Log "COULD NOT SWITCH BACK to ${ownerRef}: $($_.Exception.Message)"; $exitCode = 2 }
    }
    Remove-Item $lock -ErrorAction SilentlyContinue
    Log "exit $exitCode"
}
exit $exitCode
