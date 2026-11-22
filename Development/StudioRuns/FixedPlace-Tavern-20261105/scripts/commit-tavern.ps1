#requires -Version 7.0
[CmdletBinding()]
param(
    [switch]$Commit,
    [string]$ManifestPath,
    [string]$ManifestSHA256,
    [string]$MessageFile
)
$ErrorActionPreference = 'Stop'
$repo = 'D:\AI_Agent_Lab\rrpg_pilot\repo'
$gitExe = (Get-Command git.exe -ErrorAction Stop).Source
$attemptDir = $null

# Only fixed Git operations are called below. This is not a shell broker.
function Invoke-TaskGit {
    param([string[]]$GitArgs, [int]$TimeoutSeconds = 30)
    $psi = [Diagnostics.ProcessStartInfo]::new()
    $psi.FileName = $gitExe
    $psi.WorkingDirectory = $repo
    $psi.UseShellExecute = $false
    $psi.CreateNoWindow = $true
    $psi.RedirectStandardOutput = $true
    $psi.RedirectStandardError = $true
    $psi.Environment['GIT_OPTIONAL_LOCKS'] = '0'
    $psi.Environment['GIT_TERMINAL_PROMPT'] = '0'
    foreach ($a in $GitArgs) { $psi.ArgumentList.Add($a) }
    $p = [Diagnostics.Process]::new()
    $p.StartInfo = $psi
    try {
        if (!$p.Start()) { throw 'Git failed to start' }
        $stdout = $p.StandardOutput.ReadToEndAsync()
        $stderr = $p.StandardError.ReadToEndAsync()
        $exited = $p.WaitForExit($TimeoutSeconds * 1000)
        if (!$exited) {
            try { $p.Kill($true) } catch { throw 'Git timed out and termination is unconfirmed; preserve lock and inspect this attempt' }
            if (!$p.WaitForExit(5000)) { throw 'Git exit remains unknown; preserve lock and inspect this attempt' }
        }
        if (!$stdout.Wait(5000) -or !$stderr.Wait(5000)) { throw 'Git output did not close; preserve state and inspect this attempt' }
        $out = $stdout.Result
        $err = $stderr.Result
        if ($attemptDir) {
            @{ args = $GitArgs; exitCode = $p.ExitCode; timedOut = !$exited; stdout = $out; stderr = $err } |
                ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $attemptDir (([Guid]::NewGuid().ToString('N')) + '.json')) -Encoding utf8
        }
        if (!$exited) { throw 'Git timed out; owned process tree stopped. Inspect logs and index; do not replay or delete locks blindly' }
        if ($p.ExitCode -ne 0) { throw ('Git exit ' + $p.ExitCode + ': ' + $err.Trim()) }
        return $out
    } finally { $p.Dispose() }
}
function Assert-ProjectFile([string]$Value) {
    if ([string]::IsNullOrWhiteSpace($Value)) { throw 'Missing file path' }
    $full = [IO.Path]::GetFullPath($Value, $repo)
    if (!$full.StartsWith($repo + '\', [StringComparison]::OrdinalIgnoreCase)) { throw 'File outside exact Pilot repository' }
    $relative = [IO.Path]::GetRelativePath($repo, $full).Replace('\', '/')
    if ($relative -match '(^|/)(\.git|\.codex|\.runtime)(/|$)') { throw 'Private or Git metadata cannot be a submitted file' }
    $cursor = $repo
    foreach ($part in $relative.Split('/')) {
        $cursor = Join-Path $cursor $part
        $item = Get-Item -LiteralPath $cursor -Force
        if ($item.Attributes -band [IO.FileAttributes]::ReparsePoint) { throw 'Reparse points are not accepted' }
    }
    if (!(Test-Path -LiteralPath $full -PathType Leaf)) { throw 'Only exact existing files are accepted; directories are forbidden' }
    return $full
}

$head = (Invoke-TaskGit -GitArgs @('rev-parse', 'HEAD')).Trim()
$branch = (Invoke-TaskGit -GitArgs @('symbolic-ref', '--short', 'HEAD')).Trim()
if (!$ManifestPath) {
    if ($Commit) { throw 'No writes: -Commit requires an explicitly authorized exact-file manifest, its SHA256, and a message file' }
    Write-Output ('MODE=PREVIEW; HEAD=' + $head + '; BRANCH=' + $branch)
    Write-Output 'No staging, commit, lock removal, permission changes, or editor execution. Supply an authorized manifest to review exact files.'
    return
}
$manifestFile = Assert-ProjectFile $ManifestPath
$manifest = Get-Content -LiteralPath $manifestFile -Raw | ConvertFrom-Json
if ($manifest.version -ne 1 -or $manifest.repository -ne $repo -or $manifest.head -ne $head -or $manifest.branch -ne $branch) {
    throw 'Manifest repository/HEAD/branch differs from current workspace'
}
$rows = @($manifest.files)
if ($rows.Count -lt 1 -or $rows.Count -gt 1000) { throw 'Manifest must contain 1..1000 exact files' }
$paths = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach ($row in $rows) {
    $full = Assert-ProjectFile $row.path
    $relative = [IO.Path]::GetRelativePath($repo, $full).Replace('\', '/')
    if ($row.path -cne $relative -or !$paths.Add($relative)) { throw 'Use unique canonical repository-relative file paths' }
    if ($row.sha256 -notmatch '^[a-fA-F0-9]{64}$' -or (Get-FileHash -LiteralPath $full -Algorithm SHA256).Hash -ne $row.sha256) {
        throw ('Manifest file changed or hash missing: ' + $relative)
    }
}
$manifestHash = (Get-FileHash -LiteralPath $manifestFile -Algorithm SHA256).Hash
Write-Output ('MODE=' + $(if ($Commit) { 'EXPLICIT_COMMIT' } else { 'PREVIEW' }) + '; FILES=' + $rows.Count + '; MANIFEST_SHA256=' + $manifestHash)
foreach ($file in $paths) { Write-Output ('FILE=' + $file) }
if (!$Commit) { Write-Output 'No Git writes performed. Preview is not approval.'; return }
if ($ManifestSHA256 -notmatch '^[a-fA-F0-9]{64}$' -or $ManifestSHA256 -ne $manifestHash) { throw 'Reviewed manifest SHA256 required; no writes performed' }
$messagePath = Assert-ProjectFile $MessageFile
if ([string]::IsNullOrWhiteSpace((Get-Content -LiteralPath $messagePath -Raw))) { throw 'Commit message file is empty' }
if (Test-Path -LiteralPath (Join-Path $repo '.git\index.lock')) { throw 'Index lock exists. Do not remove it here; inspect owner and lifecycle first' }
if ((Invoke-TaskGit -GitArgs @('diff', '--cached', '--name-only', '-z')).Length -ne 0) { throw 'Pre-existing staged changes; no staging allowed' }

# Explicit invocation and hashes prevent mistakes; they do not prove user authorization.
# The invoking agent must hold the actual approval for the manifest's full scope.
$attemptDir = Join-Path $repo ('Development\StudioRuns\FixedPlace-Tavern-20261105\git-attempts\' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $attemptDir | Out-Null
Copy-Item -LiteralPath $manifestFile -Destination (Join-Path $attemptDir 'manifest.json')
$pathspec = Join-Path $attemptDir 'pathspec.nul'
[IO.File]::WriteAllBytes($pathspec, [Text.UTF8Encoding]::new($false).GetBytes((($paths | Sort-Object) -join [char]0) + [char]0))
$null = Invoke-TaskGit -GitArgs @('--literal-pathspecs', 'add', ('--pathspec-from-file=' + $pathspec), '--pathspec-file-nul') -TimeoutSeconds 60
$staged = @((Invoke-TaskGit -GitArgs @('diff', '--cached', '--name-only', '-z')).Split([char]0, [StringSplitOptions]::RemoveEmptyEntries))
if ($staged.Count -ne $paths.Count -or @($staged | Where-Object { !$paths.Contains($_) }).Count) {
    throw 'Actual staged paths differ from manifest; preserve index and review, do not automatically reset'
}
if ((Invoke-TaskGit -GitArgs @('rev-parse', 'HEAD')).Trim() -ne $head) { throw 'HEAD changed; preserve staged state for review' }
foreach ($row in $rows) {
    if ((Get-FileHash -LiteralPath (Join-Path $repo $row.path) -Algorithm SHA256).Hash -ne $row.sha256) { throw 'Working file changed during staging; preserve state for review' }
}
$null = Invoke-TaskGit -GitArgs @('diff', '--cached', '--check')
$null = Invoke-TaskGit -GitArgs @('commit', '-F', $messagePath) -TimeoutSeconds 60
Write-Output ('SHA=' + (Invoke-TaskGit -GitArgs @('rev-parse', 'HEAD')).Trim())
Write-Output 'Commit attempted only for the explicit manifest. No Push, hook bypass, ACL changes, or automatic rollback.'
