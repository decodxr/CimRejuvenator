$ErrorActionPreference = "Stop"
$env:DOTNET_ROLL_FORWARD = "LatestMajor"

Write-Host "=== Cim Rejuvenator - build Release ==="

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "dotnet nao encontrado. Rode .\check-environment.ps1"
}

if ([string]::IsNullOrWhiteSpace($env:CSII_TOOLPATH)) {
    throw "CSII_TOOLPATH nao encontrado. Rode .\check-environment.ps1"
}

if (-not (Test-Path "$env:CSII_TOOLPATH\Mod.props")) {
    throw "Mod.props nao encontrado. Rode .\check-environment.ps1"
}

if (-not (Test-Path "$env:CSII_TOOLPATH\Mod.targets")) {
    throw "Mod.targets nao encontrado. Rode .\check-environment.ps1"
}

dotnet build .\CimRejuvenator.csproj -c Release

if ($LASTEXITCODE -ne 0) {
    throw "A compilacao falhou. Para salvar o log: .\build.ps1 *> build-error.txt"
}

Write-Host ""
Write-Host "Build finalizado. Agora abra o Cities: Skylines II e procure Cim Rejuvenator em Opcoes > Mods." -ForegroundColor Green
