param(
  [Parameter(Mandatory=$true, Position=0)]
  [ValidateSet('capture','verify','begin','end','selftest','status','classify')]
  [string]$Command,
  [string]$Label = 'manual',
  [string]$PathOverride,
  [string]$OutRootOverride,
  [string]$OldPath,
  [switch]$Force
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'font-guard-core.ps1')

try {
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$runDir    = Split-Path -Parent $scriptDir
$repo      = (Resolve-Path (Join-Path $runDir '..\..\..')).Path
$fontRel   = 'UnityProject/RRPG1.0game/Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset'
if ($PathOverride) { $fontPath = $PathOverride } else { $fontPath = Join-Path $repo ($fontRel -replace '/', '\') }
if ($OutRootOverride) { $outRoot = $OutRootOverride } else { $outRoot = Join-Path $runDir 'font-protection' }

$baselineJson = Join-Path $outRoot 'baseline.json'
$backupDir    = Join-Path $outRoot 'baseline'
$backupFile   = Join-Path $backupDir 'RRPG_SourceHanSansCN_Regular_TMP.asset'
$recordsDir   = Join-Path $outRoot 'records'
$incidentDir  = Join-Path $outRoot 'incidents'
$growthDir    = Join-Path $outRoot 'growth'
$historyDir   = Join-Path $outRoot 'history'
$utf8 = New-Object System.Text.UTF8Encoding($false)

function Write-Utf8([string]$path, [string]$text) {
  $dir = Split-Path -Parent $path
  if ($dir -and -not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
  [System.IO.File]::WriteAllText($path, $text, $utf8)
}
function Get-FileState([string]$p) {
  if (-not (Test-Path -LiteralPath $p)) { return @{ exists = $false; bytes = 0; sha256 = ''; mtimeUtc = '' } }
  $i = Get-Item -LiteralPath $p
  return @{ exists = $true; bytes = [int64]$i.Length; sha256 = (Get-FileHash -LiteralPath $p -Algorithm SHA256).Hash.ToLower(); mtimeUtc = $i.LastWriteTimeUtc.ToString('o') }
}
function NowStamp { (Get-Date).ToUniversalTime().ToString('yyyyMMddTHHmmssZ') }
function SafeLabel([string]$s) { return ($s -replace '[^\w\-]', '_') }
function Load-Baseline {
  if (-not (Test-Path -LiteralPath $baselineJson)) { throw 'BASELINE_MISSING: run capture first' }
  return (Get-Content -LiteralPath $baselineJson -Raw -Encoding UTF8 | ConvertFrom-Json)
}
function Emit($obj) { $obj | ConvertTo-Json -Depth 10 -Compress }

function Adopt-NewBaseline($b, $cur, $verdict, [string]$label) {
  $stamp = NowStamp
  New-Item -ItemType Directory -Force -Path $backupDir, $growthDir, $historyDir | Out-Null
  $histFile = Join-Path $historyDir (($b.sha256) + '.asset')
  if ((-not (Test-Path -LiteralPath $histFile)) -and (Test-Path -LiteralPath $backupFile)) { Copy-Item -LiteralPath $backupFile -Destination $histFile -Force }
  Copy-Item -LiteralPath $fontPath -Destination $backupFile -Force
  $bk = Get-FileState $backupFile
  if ($bk.sha256 -ne $cur.sha256) { throw 'BACKUP_READBACK_MISMATCH' }
  $rec = [ordered]@{
    schema = 'rrpg-font-protection-growth/1'
    label = $label
    adoptedUtc = (Get-Date).ToUniversalTime().ToString('o')
    previous = @{ sha256 = $b.sha256; bytes = $b.bytes }
    current  = @{ sha256 = $cur.sha256; bytes = $cur.bytes; mtimeUtc = $cur.mtimeUtc }
    deltaBytes = ($cur.bytes - [int64]$b.bytes)
    verdict = $verdict.Verdict
    detail = $verdict.Detail
    policy = 'legitimate dynamic atlas growth: additive characters/glyphs only; adopted automatically'
  }
  $recPath = Join-Path $growthDir ($stamp + '-' + (SafeLabel $label) + '.json')
  Write-Utf8 $recPath ($rec | ConvertTo-Json -Depth 12)
  $obj = [ordered]@{
    schema = 'rrpg-font-protection-baseline/1'
    adoptedUtc = (Get-Date).ToUniversalTime().ToString('o')
    decision = $b.decision
    fontPath = $fontRel
    sha256 = $cur.sha256
    bytes = $cur.bytes
    mtimeUtc = $cur.mtimeUtc
    backupFile = 'baseline/RRPG_SourceHanSansCN_Regular_TMP.asset'
    backupSha256 = $bk.sha256
    backupBytes = $bk.bytes
    backupReadbackOk = $true
    previousStates = $b.previousStates
    policy = 'PASS = identical; LEGITIMATE_GROWTH = additive dynamic atlas only (auto-adopted, recorded); INCIDENT/NEEDS_REVIEW = stop, no auto-restore'
  }
  Write-Utf8 $baselineJson ($obj | ConvertTo-Json -Depth 10)
  return $recPath
}

# 判定当前字体相对基线：PASS / LEGITIMATE_GROWTH（自动采纳）/ INCIDENT / NEEDS_REVIEW
function Test-Drift([string]$label, [switch]$Adopt) {
  $b = Load-Baseline
  $cur = Get-FileState $fontPath
  if (-not $cur.exists) { return [pscustomobject]@{ Verdict = 'MISSING'; Baseline = $b; Current = $cur } }
  if ($cur.sha256 -eq $b.sha256 -and $cur.bytes -eq [int64]$b.bytes) {
    return [pscustomobject]@{ Verdict = 'PASS'; Baseline = $b; Current = $cur }
  }
  if (-not (Test-Path -LiteralPath $backupFile)) {
    return [pscustomobject]@{ Verdict = 'NEEDS_REVIEW'; Baseline = $b; Current = $cur; Reason = 'baseline backup missing' }
  }
  $v = Get-FontDriftVerdict $backupFile $fontPath
  if ($v.Verdict -eq 'LEGITIMATE_GROWTH' -and $Adopt) {
    $recPath = Adopt-NewBaseline $b $cur $v $label
    $v.Detail['growthRecord'] = $recPath
  }
  return [pscustomobject]@{ Verdict = $v.Verdict; Baseline = $b; Current = $cur; Detail = $v.Detail }
}

function Write-Incident([string]$label, $result, [string]$kind) {
  $stamp = NowStamp
  $dir = Join-Path $incidentDir ($stamp + '-' + (SafeLabel $label))
  New-Item -ItemType Directory -Force -Path $dir | Out-Null
  $rec = [ordered]@{
    schema = 'rrpg-font-protection-incident/1'
    kind = $kind
    detectedUtc = (Get-Date).ToUniversalTime().ToString('o')
    label = $label
    fontPath = $fontPath
    baseline = @{ sha256 = $result.Baseline.sha256; bytes = $result.Baseline.bytes }
    current = @{ sha256 = $result.Current.sha256; bytes = $result.Current.bytes; mtimeUtc = $result.Current.mtimeUtc }
    reason = $result.Detail
    action = 'STOP: no restore, no git overwrite, evidence preserved'
  }
  Write-Utf8 (Join-Path $dir 'incident.json') ($rec | ConvertTo-Json -Depth 12)
  if ($result.Current.exists) { Copy-Item -LiteralPath $fontPath -Destination (Join-Path $dir 'font-after.asset') -Force }
  if (Test-Path -LiteralPath $backupFile) { Copy-Item -LiteralPath $backupFile -Destination (Join-Path $dir 'font-baseline.asset') -Force }
  return $dir
}

switch ($Command) {

  'classify' {
    if (-not $OldPath) { throw 'OLD_REQUIRED: -OldPath <file>' }
    $v = Get-FontDriftVerdict $OldPath $fontPath
    Emit @{ command = 'classify'; old = $OldPath; new = $fontPath; verdict = $v.Verdict; detail = $v.Detail }
    if ($v.Verdict -eq 'PASS') { exit 0 }
    if ($v.Verdict -eq 'LEGITIMATE_GROWTH') { exit 0 }
    if ($v.Verdict -eq 'NEEDS_REVIEW') { exit 4 }
    exit 2
  }

  'capture' {
    if ((Test-Path -LiteralPath $baselineJson) -and -not $Force) { throw 'BASELINE_EXISTS: refusing to overwrite without -Force' }
    $st = Get-FileState $fontPath
    if (-not $st.exists) { throw ('FONT_MISSING: ' + $fontPath) }
    New-Item -ItemType Directory -Force -Path $backupDir | Out-Null
    Copy-Item -LiteralPath $fontPath -Destination $backupFile -Force
    $bk = Get-FileState $backupFile
    if ($bk.sha256 -ne $st.sha256 -or $bk.bytes -ne $st.bytes) { throw 'BACKUP_READBACK_MISMATCH' }
    $obj = [ordered]@{
      schema = 'rrpg-font-protection-baseline/1'
      adoptedUtc = (Get-Date).ToUniversalTime().ToString('o')
      decision = 'Producer 2026-11-05: accept legitimate dynamic atlas growth; no static-font swap, no runtime clone'
      fontPath = $fontRel
      sha256 = $st.sha256; bytes = $st.bytes; mtimeUtc = $st.mtimeUtc
      backupFile = 'baseline/RRPG_SourceHanSansCN_Regular_TMP.asset'
      backupSha256 = $bk.sha256; backupBytes = $bk.bytes; backupReadbackOk = $true
      previousStates = @(
        @{ when = '2026-10-22'; bytes = 14961033; sha256 = '3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854' },
        @{ when = '2026-10-23T12:30:59Z'; bytes = 14968490; sha256 = 'e40b6d77a93712ee5371d35311051edfeb50f730d4d07ef5e03bd7dd7556f2d8' },
        @{ when = '2026-11-01T05:36:25Z'; bytes = 14968844; sha256 = '9ada53620a042f0fc823a1301e7cc5692d847a4f97845bf78be5ce342b1c22d5' },
        @{ when = '2026-11-05T08:03:25Z'; bytes = 14972511; sha256 = 'd19fd6a1672ccccc05b3fbc842eeb93d327a5727ad6d65e85493170f6d317fee' }
      )
      policy = 'PASS = identical; LEGITIMATE_GROWTH = additive dynamic atlas only (auto-adopted, recorded); INCIDENT/NEEDS_REVIEW = stop, no auto-restore'
    }
    Write-Utf8 $baselineJson ($obj | ConvertTo-Json -Depth 10)
    Emit @{ command = 'capture'; result = 'BASELINE_ADOPTED'; sha256 = $st.sha256; bytes = $st.bytes; backup = $backupFile }
    exit 0
  }

  'status' {
    $b = Load-Baseline
    $cur = Get-FileState $fontPath
    $same = ($cur.exists -and $cur.sha256 -eq $b.sha256)
    Emit @{ command = 'status'; baselineSha = $b.sha256; currentSha = $cur.sha256; currentBytes = $cur.bytes; match = $same }
    if ($same) { exit 0 } elseif ($cur.exists) { exit 2 } else { exit 3 }
  }

  'verify' {
    $r = Test-Drift $Label -Adopt
    switch ($r.Verdict) {
      'PASS' { Emit @{ command = 'verify'; result = 'PASS'; sha256 = $r.Current.sha256; bytes = $r.Current.bytes }; exit 0 }
      'LEGITIMATE_GROWTH' { Emit @{ command = 'verify'; result = 'LEGITIMATE_GROWTH'; sha256 = $r.Current.sha256; bytes = $r.Current.bytes; detail = $r.Detail }; exit 0 }
      'NEEDS_REVIEW' { $d = Write-Incident $Label $r 'needs-review'; Emit @{ command = 'verify'; result = 'NEEDS_REVIEW'; reason = $r.Detail; evidenceDir = $d }; exit 4 }
      default { $d = Write-Incident $Label $r 'incident'; Emit @{ command = 'verify'; result = 'INCIDENT'; reason = $r.Detail; evidenceDir = $d }; exit 2 }
    }
  }

  'begin' {
    $r = Test-Drift $Label -Adopt
    switch ($r.Verdict) {
      'PASS' { $stamp = NowStamp; $name = $stamp + '-' + (SafeLabel $Label) + '-begin.json'
        Write-Utf8 (Join-Path $recordsDir $name) (@{ schema = 'rrpg-font-protection-record/1'; label = $Label; beganUtc = (Get-Date).ToUniversalTime().ToString('o'); preSha = $r.Current.sha256 } | ConvertTo-Json -Depth 6)
        Emit @{ command = 'begin'; result = 'OK'; sha256 = $r.Current.sha256; record = ('records/' + $name); growthAdopted = $false }; exit 0 }
      'LEGITIMATE_GROWTH' { Emit @{ command = 'begin'; result = 'OK_AFTER_GROWTH_ADOPTED'; sha256 = $r.Current.sha256; growthRecord = $r.Detail['growthRecord'] }; exit 0 }
      'NEEDS_REVIEW' { $d = Write-Incident $Label $r 'needs-review'; Emit @{ command = 'begin'; result = 'NEEDS_REVIEW'; reason = $r.Detail; evidenceDir = $d }; exit 4 }
      default { $d = Write-Incident $Label $r 'incident'; Emit @{ command = 'begin'; result = 'INCIDENT'; reason = $r.Detail; evidenceDir = $d }; exit 2 }
    }
  }

  'end' {
    $r = Test-Drift $Label -Adopt
    $stamp = NowStamp
    switch ($r.Verdict) {
      'PASS' { $name = $stamp + '-' + (SafeLabel $Label) + '-end.json'
        Write-Utf8 (Join-Path $recordsDir $name) (@{ schema = 'rrpg-font-protection-record/1'; label = $Label; endedUtc = (Get-Date).ToUniversalTime().ToString('o'); postSha = $r.Current.sha256; result = 'PASS' } | ConvertTo-Json -Depth 6)
        Emit @{ command = 'end'; result = 'PASS'; sha256 = $r.Current.sha256; record = ('records/' + $name) }; exit 0 }
      'LEGITIMATE_GROWTH' { $name = $stamp + '-' + (SafeLabel $Label) + '-end-growth.json'
        Write-Utf8 (Join-Path $recordsDir $name) (@{ schema = 'rrpg-font-protection-record/1'; label = $Label; endedUtc = (Get-Date).ToUniversalTime().ToString('o'); postSha = $r.Current.sha256; result = 'LEGITIMATE_GROWTH'; detail = $r.Detail } | ConvertTo-Json -Depth 12)
        Emit @{ command = 'end'; result = 'LEGITIMATE_GROWTH_ADOPTED'; sha256 = $r.Current.sha256; record = ('records/' + $name); growthRecord = $r.Detail['growthRecord'] }; exit 0 }
      'NEEDS_REVIEW' { $d = Write-Incident $Label $r 'needs-review'; Emit @{ command = 'end'; result = 'NEEDS_REVIEW'; reason = $r.Detail; evidenceDir = $d }; exit 4 }
      default { $d = Write-Incident $Label $r 'incident'; Emit @{ command = 'end'; result = 'INCIDENT'; reason = $r.Detail; evidenceDir = $d }; exit 2 }
    }
  }

  'selftest' {
    $tmp = Join-Path $env:TEMP ('font-guard-selftest-' + [guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Force -Path $tmp | Out-Null
    try {
      $copy = Join-Path $tmp 'font-copy.asset'
      Copy-Item -LiteralPath $fontPath -Destination $copy -Force
      $out = Join-Path $tmp 'prot'
      $null = & $PSCommandPath capture -PathOverride $copy -OutRootOverride $out -Force
      $capExit = $LASTEXITCODE
      $null = & $PSCommandPath verify -PathOverride $copy -OutRootOverride $out -Label selftest-unchanged
      $e1 = $LASTEXITCODE
      $bytes = [System.IO.File]::ReadAllBytes($copy)
      $bytes[100] = [byte]([int]$bytes[100] -bxor 1)
      [System.IO.File]::WriteAllBytes($copy, $bytes)
      $null = & $PSCommandPath verify -PathOverride $copy -OutRootOverride $out -Label selftest-flip
      $e2 = $LASTEXITCODE
      $pass = ($capExit -eq 0) -and ($e1 -eq 0) -and ($e2 -eq 2)
      Emit @{ command = 'selftest'; result = $(if ($pass) { 'PASS' } else { 'FAIL' }); captureExit = $capExit; unchangedExit = $e1; corruptExit = $e2 }
      if ($pass) { exit 0 } else { exit 1 }
    }
    finally { Remove-Item -LiteralPath $tmp -Recurse -Force -ErrorAction SilentlyContinue }
  }
}

}
catch {
  $msg = 'FATAL ' + $_.Exception.GetType().Name + ': ' + $_.Exception.Message
  if ($_.InvocationInfo) { $msg += ' @line ' + $_.InvocationInfo.ScriptLineNumber }
  Write-Output $msg
  exit 1
}
