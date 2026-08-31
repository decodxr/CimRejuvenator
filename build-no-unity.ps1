$ErrorActionPreference = "Stop"
$env:DOTNET_ROLL_FORWARD = "LatestMajor"

Write-Host "=== Cim Rejuvenator - build SEM UNITY ==="
Write-Host ""

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "dotnet nao encontrado. Instale o .NET SDK primeiro."
}

# Try CSII_GAMEPATH first, then common Steam locations.
$gamePath = $env:CSII_GAMEPATH

$candidates = @(
    $gamePath,
    "C:\Program Files (x86)\Steam\steamapps\common\Cities Skylines II",
    "C:\Program Files\Steam\steamapps\common\Cities Skylines II",
    "D:\SteamLibrary\steamapps\common\Cities Skylines II",
    "E:\SteamLibrary\steamapps\common\Cities Skylines II"
) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }

$found = $null
foreach ($candidate in $candidates) {
    if (Test-Path (Join-Path $candidate "Cities2_Data\Managed\Game.dll")) {
        $found = $candidate
        break
    }
}

if ($null -eq $found) {
    Write-Host "Cities: Skylines II nao foi encontrado automaticamente." -ForegroundColor Yellow
    Write-Host "Defina manualmente, por exemplo:" -ForegroundColor Yellow
    Write-Host '$env:CSII_GAMEPATH="D:\SteamLibrary\steamapps\common\Cities Skylines II"' -ForegroundColor Cyan
    Write-Host "Depois rode .\build-no-unity.ps1 novamente."
    exit 1
}

$env:CSII_GAMEPATH = $found
Write-Host "[OK] Jogo encontrado em: $found" -ForegroundColor Green
Write-Host "[OK] Usando DLLs de: $found\Cities2_Data\Managed" -ForegroundColor Green
Write-Host "[INFO] A toolchain oficial/Unity sera IGNORADA neste build." -ForegroundColor Cyan
Write-Host ""

dotnet build .\CimRejuvenator.csproj -c Release -p:ForceNoUnityBuild=true -p:CitiesSkylines2Path="$found"

if ($LASTEXITCODE -ne 0) {
    throw "A compilacao falhou. Salve o log com: .\build-no-unity.ps1 *> build-error.txt"
}

$dll = Join-Path $PSScriptRoot "bin\Release\CimRejuvenator.dll"
$distDir = Join-Path $PSScriptRoot "dist\CimRejuvenator"

if (-not (Test-Path $dll)) {
    # Search in case SDK layout differs.
    $dll = Get-ChildItem -Path (Join-Path $PSScriptRoot "bin") -Filter "CimRejuvenator.dll" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty FullName
}

if ([string]::IsNullOrWhiteSpace($dll) -or -not (Test-Path $dll)) {
    throw "Build terminou, mas CimRejuvenator.dll nao foi localizado em bin."
}

New-Item -ItemType Directory -Force -Path $distDir | Out-Null
Copy-Item $dll (Join-Path $distDir "CimRejuvenator.dll") -Force

Write-Host ""
Write-Host "BUILD SEM UNITY CONCLUIDO!" -ForegroundColor Green
Write-Host "DLL: $dll" -ForegroundColor Green
Write-Host "Pacote para copiar: $distDir" -ForegroundColor Green
Write-Host ""
Write-Host "Para levar ao Linux, copie a pasta dist\CimRejuvenator inteira." -ForegroundColor Cyan
