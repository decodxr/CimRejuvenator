$ErrorActionPreference = "Continue"

Write-Host "=== Cim Rejuvenator - environment check ==="
Write-Host ""

$ok = $true

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if ($null -eq $dotnet) {
    Write-Host "[ERROR] dotnet was not found." -ForegroundColor Red
    Write-Host "        Install it with: winget install Microsoft.DotNet.SDK.10"
    $ok = $false
} else {
    $version = dotnet --version
    Write-Host "[OK] dotnet: $version" -ForegroundColor Green
}

$toolchainReady = $false
if (-not [string]::IsNullOrWhiteSpace($env:CSII_TOOLPATH)) {
    $props = Join-Path $env:CSII_TOOLPATH "Mod.props"
    $targets = Join-Path $env:CSII_TOOLPATH "Mod.targets"
    if ((Test-Path $props) -and (Test-Path $targets)) {
        $toolchainReady = $true
        Write-Host "[OK] Official toolchain: $env:CSII_TOOLPATH" -ForegroundColor Green
    }
}

if (-not $toolchainReady) {
    Write-Host "[INFO] The official toolchain is not ready. Direct-assembly builds can still work." -ForegroundColor Cyan
}

$gamePath = $env:CSII_GAMEPATH
$candidates = @(
    $gamePath,
    "C:\Program Files (x86)\Steam\steamapps\common\Cities Skylines II",
    "C:\Program Files\Steam\steamapps\common\Cities Skylines II",
    "D:\SteamLibrary\steamapps\common\Cities Skylines II",
    "E:\SteamLibrary\steamapps\common\Cities Skylines II"
) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }

$gameFound = $null
foreach ($candidate in $candidates) {
    if (Test-Path (Join-Path $candidate "Cities2_Data\Managed\Game.dll")) {
        $gameFound = $candidate
        break
    }
}

if ($null -ne $gameFound) {
    Write-Host "[OK] Game found for direct-assembly build: $gameFound" -ForegroundColor Green
} else {
    Write-Host "[WARNING] The game was not found automatically." -ForegroundColor Yellow
    Write-Host '          If it is installed in another library, set: $env:CSII_GAMEPATH="GAME_PATH"'
}

Write-Host ""
if ($ok -and ($toolchainReady -or $null -ne $gameFound)) {
    Write-Host "At least one build mode is ready." -ForegroundColor Green
    if ($toolchainReady) {
        Write-Host "  Official toolchain: .\build.ps1"
    }
    if ($null -ne $gameFound) {
        Write-Host "  Direct assemblies:  .\build-no-unity.ps1"
    }
    exit 0
}

Write-Host "The environment is missing the .NET SDK, the game path, or the official toolchain." -ForegroundColor Yellow
exit 1
