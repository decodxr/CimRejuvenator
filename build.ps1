$ErrorActionPreference = "Stop"
$env:DOTNET_ROLL_FORWARD = "LatestMajor"

Write-Host "=== Cim Rejuvenator - official toolchain build ==="

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "dotnet was not found. Run .\check-environment.ps1 first."
}

if ([string]::IsNullOrWhiteSpace($env:CSII_TOOLPATH)) {
    throw "CSII_TOOLPATH was not found. Run .\check-environment.ps1 first."
}

if (-not (Test-Path "$env:CSII_TOOLPATH\Mod.props")) {
    throw "Mod.props was not found under CSII_TOOLPATH."
}

if (-not (Test-Path "$env:CSII_TOOLPATH\Mod.targets")) {
    throw "Mod.targets was not found under CSII_TOOLPATH."
}

dotnet build .\CimRejuvenator.csproj -c Release

if ($LASTEXITCODE -ne 0) {
    throw "Build failed. Save a complete log with: .\build.ps1 *> build-error.txt"
}

Write-Host ""
Write-Host "Build completed successfully." -ForegroundColor Green
