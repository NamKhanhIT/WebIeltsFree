param(
    [string]$AppHost = "0.0.0.0",
    [int]$Port = 8000,
    [switch]$SkipInstall,
    [switch]$RecreateVenv
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

function Invoke-CheckedCommand {
    param(
        [string]$Description,
        [scriptblock]$Command
    )

    & $Command
    if ($LASTEXITCODE -ne 0) {
        throw "[Python AI] $Description failed (exit code $LASTEXITCODE)."
    }
}

function Parse-PythonVersion {
    param([string]$Version)

    if (-not $Version) { return $null }
    $parts = $Version.Trim() -split "\."
    if ($parts.Length -lt 2) { return $null }
    return [pscustomobject]@{
        Major = [int]$parts[0]
        Minor = [int]$parts[1]
        Raw = $Version.Trim()
    }
}

function Test-SupportedPythonVersion {
    param($VersionInfo)
    return $VersionInfo -and $VersionInfo.Major -eq 3 -and $VersionInfo.Minor -ge 10
}

function Get-PreferredPythonLauncher {
    $candidates = @()

    if (Get-Command py -ErrorAction SilentlyContinue) {
        # Prefer stable interpreter versions with broad wheel support.
        $candidates += @(
            @{ Exe = "py"; PrefixArgs = @("-3.12") },
            @{ Exe = "py"; PrefixArgs = @("-3.11") },
            @{ Exe = "py"; PrefixArgs = @("-3.10") },
            @{ Exe = "py"; PrefixArgs = @("-3") }
        )
    }

    if (Get-Command python -ErrorAction SilentlyContinue) {
        $candidates += @(
            @{ Exe = "python"; PrefixArgs = @() }
        )
    }

    foreach ($candidate in $candidates) {
        try {
            $versionRaw = & $candidate.Exe @($candidate.PrefixArgs + @("-c", "import sys; print(f'{sys.version_info.major}.{sys.version_info.minor}')")) 2>$null
            $version = Parse-PythonVersion ($versionRaw | Select-Object -First 1)
            if (Test-SupportedPythonVersion $version) {
                return @{
                    Exe = $candidate.Exe
                    PrefixArgs = $candidate.PrefixArgs
                    Version = $version.Raw
                }
            }
        } catch {
            continue
        }
    }

    throw "Python 3.10+ not found. Install Python 3.10, 3.11, or 3.12 and retry."
}

function Get-VenvPythonVersion {
    if (-not (Test-Path ".venv\Scripts\python.exe")) {
        return $null
    }

    try {
        $versionRaw = & ".venv\Scripts\python.exe" -c "import sys; print(f'{sys.version_info.major}.{sys.version_info.minor}')" 2>$null
        return Parse-PythonVersion ($versionRaw | Select-Object -First 1)
    } catch {
        return $null
    }
}

function New-VenvIfNeeded {
    $launcher = Get-PreferredPythonLauncher
    $venvVersion = Get-VenvPythonVersion

    if ($RecreateVenv -and (Test-Path ".venv")) {
        Write-Host "[Python AI] Recreating virtual environment (--RecreateVenv)..." -ForegroundColor Yellow
        Remove-Item ".venv" -Recurse -Force
        $venvVersion = $null
    }

    if ($venvVersion -and -not (Test-SupportedPythonVersion $venvVersion)) {
        Write-Host "[Python AI] Existing .venv uses unsupported Python $($venvVersion.Raw). Recreating with Python $($launcher.Version)..." -ForegroundColor Yellow
        Remove-Item ".venv" -Recurse -Force
        $venvVersion = $null
    }

    if (Test-Path ".venv\Scripts\python.exe") {
        Write-Host "[Python AI] Using existing virtual environment." -ForegroundColor Cyan
        return
    }

    Write-Host "[Python AI] Creating virtual environment with Python $($launcher.Version)..." -ForegroundColor Cyan
    Invoke-CheckedCommand "Virtual environment creation" {
        & $launcher.Exe @($launcher.PrefixArgs + @("-m", "venv", ".venv"))
    }
}

New-VenvIfNeeded

$venvPython = Resolve-Path ".venv\Scripts\python.exe"

if (-not $SkipInstall) {
    Write-Host "[Python AI] Installing requirements..." -ForegroundColor Cyan
    Invoke-CheckedCommand "Pip upgrade" {
        & $venvPython -m pip install --upgrade pip
    }

    Invoke-CheckedCommand "Requirements installation" {
        & $venvPython -m pip install --prefer-binary -r requirements.txt
    }
}

if (-not $env:GEMINI_API_KEY) {
    Write-Host "[Warning] GEMINI_API_KEY is not set. Service will run in fallback mode." -ForegroundColor Yellow
}

if (-not $env:DID_API_KEY -and -not $env:DID_API_KEY_B64) {
    Write-Host "[Warning] DID_API_KEY / DID_API_KEY_B64 is not set. Avatar provider mode is disabled." -ForegroundColor Yellow
}

if (-not $env:DID_SOURCE_URL) {
    Write-Host "[Warning] DID_SOURCE_URL is not set. Provider-generated avatar video is disabled." -ForegroundColor Yellow
}

if (-not $env:DID_AVATAR_URL) {
    Write-Host "[Warning] DID_AVATAR_URL is not set. No default fallback avatar URL is configured." -ForegroundColor Yellow
}

Write-Host "[Python AI] Starting service on http://$AppHost`:$Port ..." -ForegroundColor Green
& $venvPython -m uvicorn app.main:app --host $AppHost --port $Port --reload
