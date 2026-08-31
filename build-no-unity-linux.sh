#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DEPLOY=false

if [[ "${1:-}" == "--deploy" ]]; then
  DEPLOY=true
fi

echo "=== Cim Rejuvenator - build SEM UNITY no Linux ==="
echo

if ! command -v dotnet >/dev/null 2>&1; then
  echo "[ERRO] dotnet nao encontrado."
  echo "No Arch/Caelestia: sudo pacman -S dotnet-sdk"
  echo "Depois confirme com: dotnet --version"
  exit 1
fi

echo "[OK] dotnet: $(dotnet --version)"

GAME_PATH="${CSII_GAMEPATH:-}"

candidates=(
  "$GAME_PATH"
  "$HOME/.local/share/Steam/steamapps/common/Cities Skylines II"
  "$HOME/.steam/steam/steamapps/common/Cities Skylines II"
  "$HOME/.var/app/com.valvesoftware.Steam/.local/share/Steam/steamapps/common/Cities Skylines II"
)

FOUND=""
for candidate in "${candidates[@]}"; do
  [[ -z "$candidate" ]] && continue
  if [[ -f "$candidate/Cities2_Data/Managed/Game.dll" ]]; then
    FOUND="$candidate"
    break
  fi
done

if [[ -z "$FOUND" ]]; then
  echo "[ERRO] Cities: Skylines II nao foi encontrado automaticamente."
  echo "Defina o caminho manualmente, por exemplo:"
  echo 'export CSII_GAMEPATH="$HOME/.local/share/Steam/steamapps/common/Cities Skylines II"'
  echo "Depois rode novamente: ./build-no-unity-linux.sh"
  exit 1
fi

echo "[OK] Jogo encontrado: $FOUND"
echo "[OK] Assemblies: $FOUND/Cities2_Data/Managed"
echo "[INFO] Unity/toolchain oficial nao sera usada."
echo

export CSII_GAMEPATH="$FOUND"

dotnet build "$ROOT/CimRejuvenator.csproj" \
  -c Release \
  -p:ForceNoUnityBuild=true \
  -p:CitiesSkylines2Path="$FOUND"

DLL=""
if [[ -f "$ROOT/bin/Release/CimRejuvenator.dll" ]]; then
  DLL="$ROOT/bin/Release/CimRejuvenator.dll"
else
  DLL="$(find "$ROOT/bin" -type f -name 'CimRejuvenator.dll' -print -quit 2>/dev/null || true)"
fi

if [[ -z "$DLL" || ! -f "$DLL" ]]; then
  echo "[ERRO] Build terminou, mas CimRejuvenator.dll nao foi localizado."
  exit 1
fi

DIST="$ROOT/dist/CimRejuvenator"
mkdir -p "$DIST"
cp -f "$DLL" "$DIST/CimRejuvenator.dll"

echo
echo "[OK] BUILD CONCLUIDO"
echo "DLL: $DLL"
echo "Pacote: $DIST/CimRejuvenator.dll"

if $DEPLOY; then
  proton_candidates=(
    "$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II"
    "$HOME/.steam/steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II"
    "$HOME/.var/app/com.valvesoftware.Steam/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II"
  )

  USER_DATA=""
  for candidate in "${proton_candidates[@]}"; do
    if [[ -d "$candidate" ]]; then
      USER_DATA="$candidate"
      break
    fi
  done

  if [[ -z "$USER_DATA" ]]; then
    echo "[ERRO] Prefixo Proton do Cities: Skylines II nao encontrado para deploy automatico."
    echo "A DLL compilada continua disponivel em: $DIST/CimRejuvenator.dll"
    exit 1
  fi

  MOD_DIR="$USER_DATA/Mods/CimRejuvenator"
  mkdir -p "$MOD_DIR"
  cp -f "$DIST/CimRejuvenator.dll" "$MOD_DIR/CimRejuvenator.dll"

  echo "[OK] DEPLOY CONCLUIDO"
  echo "Instalado em: $MOD_DIR/CimRejuvenator.dll"
  echo "Feche e abra o jogo para carregar a nova DLL."
fi
