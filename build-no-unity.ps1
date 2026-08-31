$ErrorActionPreference = "Stop"
$env:DOTNET_ROLL_FORWARD = "LatestMajor"

Write-Host "=== Cim Rejuvenator - direct assembly build (Windows) ==="
Write-Host ""

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "dotnet was not found. Install the .NET SDK first."
}

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
    Write-Host "Cities: Skylines II was not found automatically." -ForegroundColor Yellow
    Write-Host "Set the path manually, for example:" -ForegroundColor Yellow
    Write-Host '$env:CSII_GAMEPATH="D:\SteamLibrary\steamapps\common\Cities Skylines II"' -ForegroundColor Cyan
    Write-Host "Then run .\build-no-unity.ps1 again."
    exit 1
}

$env:CSII_GAMEPATH = $found
Write-Host "[OK] Game: $found" -ForegroundColor Green
Write-Host "[OK] Assemblies: $found\Cities2_Data\Managed" -ForegroundColor Green
Write-Host "[INFO] The Unity editor and official modding toolchain are not used by this build." -ForegroundColor Cyan
Write-Host ""

dotnet build .\CimRejuvenator.csproj -c Release -p:ForceNoUnityBuild=true -p:CitiesSkylines2Path="$found"

if ($LASTEXITCODE -ne 0) {
    throw "Build failed. Save a complete log with: .\build-no-unity.ps1 *> build-error.txt"
}

$dll = Join-Path $PSScriptRoot "bin\Release\CimRejuvenator.dll"
$distDir = Join-Path $PSScriptRoot "dist\CimRejuvenator"

if (-not (Test-Path $dll)) {
    $dll = Get-ChildItem -Path (Join-Path $PSScriptRoot "bin") -Filter "CimRejuvenator.dll" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty FullName
}

if ([string]::IsNullOrWhiteSpace($dll) -or -not (Test-Path $dll)) {
    throw "The build completed but CimRejuvenator.dll could not be located under bin."
}

New-Item -ItemType Directory -Force -Path $distDir | Out-Null
Copy-Item $dll (Join-Path $distDir "CimRejuvenator.dll") -Force

Write-Host ""
Write-Host "BUILD COMPLETE" -ForegroundColor Green
Write-Host "DLL: $dll" -ForegroundColor Green
Write-Host "Package: $distDir" -ForegroundColor Green
