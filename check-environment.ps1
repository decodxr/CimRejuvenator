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

if ([string]::IsNullOrWhiteSpace($env:CSII_TOOLPATH)) {
    Write-Host "[ERRO] CSII_TOOLPATH esta vazio." -ForegroundColor Red
    Write-Host "       Instale/atualize a Code Modding Toolchain do Cities: Skylines II e reinicie o Windows."
    $ok = $false
} else {
    Write-Host "[OK] CSII_TOOLPATH: $env:CSII_TOOLPATH" -ForegroundColor Green

    $props = Join-Path $env:CSII_TOOLPATH "Mod.props"
    $targets = Join-Path $env:CSII_TOOLPATH "Mod.targets"

    if (Test-Path $props) {
        Write-Host "[OK] Mod.props encontrado" -ForegroundColor Green
    } else {
        Write-Host "[ERRO] Mod.props nao encontrado em: $props" -ForegroundColor Red
        $ok = $false
    }

    if (Test-Path $targets) {
        Write-Host "[OK] Mod.targets encontrado" -ForegroundColor Green
    } else {
        Write-Host "[ERRO] Mod.targets nao encontrado em: $targets" -ForegroundColor Red
        $ok = $false
    }
}

Write-Host ""
if ($ok) {
    Write-Host "Ambiente parece pronto para compilar." -ForegroundColor Green
    exit 0
} else {
    Write-Host "Existem problemas acima. Corrija-os antes de rodar build.ps1." -ForegroundColor Yellow
    exit 1
}
