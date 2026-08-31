$ErrorActionPreference = "Continue"

Write-Host "=== Cim Rejuvenator - verificador de ambiente ==="
Write-Host ""

$ok = $true

$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if ($null -eq $dotnet) {
    Write-Host "[ERRO] dotnet nao foi encontrado." -ForegroundColor Red
    Write-Host "       Instale com: winget install Microsoft.DotNet.SDK.10"
    $ok = $false
} else {
    $version = dotnet --version
    Write-Host "[OK] dotnet encontrado: $version" -ForegroundColor Green
}

$toolchainReady = $false
if (-not [string]::IsNullOrWhiteSpace($env:CSII_TOOLPATH)) {
    $props = Join-Path $env:CSII_TOOLPATH "Mod.props"
    $targets = Join-Path $env:CSII_TOOLPATH "Mod.targets"
    if ((Test-Path $props) -and (Test-Path $targets)) {
        $toolchainReady = $true
        Write-Host "[OK] Toolchain oficial encontrada em: $env:CSII_TOOLPATH" -ForegroundColor Green
    }
}

if (-not $toolchainReady) {
    Write-Host "[INFO] Toolchain oficial nao esta pronta. Isso NAO impede o build sem Unity." -ForegroundColor Cyan
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
    Write-Host "[OK] Jogo encontrado para build sem Unity: $gameFound" -ForegroundColor Green
} else {
    Write-Host "[AVISO] Jogo nao foi localizado automaticamente para o build sem Unity." -ForegroundColor Yellow
    Write-Host '        Se estiver em outra unidade, defina: $env:CSII_GAMEPATH="CAMINHO_DO_JOGO"'
}

Write-Host ""
if ($ok -and ($toolchainReady -or $null -ne $gameFound)) {
    Write-Host "Ambiente pronto para pelo menos um modo de compilacao." -ForegroundColor Green
    if ($toolchainReady) {
        Write-Host "  Oficial: .\build.ps1"
    }
    if ($null -ne $gameFound) {
        Write-Host "  Sem Unity: .\build-no-unity.ps1"
    }
    exit 0
} else {
    Write-Host "Ainda falta o .NET ou o caminho do jogo/toolchain." -ForegroundColor Yellow
    exit 1
}
