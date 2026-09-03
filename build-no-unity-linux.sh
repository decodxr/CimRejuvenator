#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DEPLOY=false

if [[ "${1:-}" == "--deploy" ]]; then
  DEPLOY=true
fi

echo "=== Cim Rejuvenator - direct assembly build (Linux) ==="
echo

if ! command -v dotnet >/dev/null 2>&1; then
  echo "[ERROR] dotnet was not found."
  echo "Arch Linux: sudo pacman -S dotnet-sdk"
  echo "Then verify the installation with: dotnet --version"
  exit 1
fi

echo "[OK] dotnet: $(dotnet --version)"

VERSION="$(sed -n 's:.*<Version>\([^<]*\)</Version>.*:\1:p' "$ROOT/CimRejuvenator.csproj" | head -n 1)"
COMMIT="unknown"
if command -v git >/dev/null 2>&1 && git -C "$ROOT" rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  COMMIT="$(git -C "$ROOT" rev-parse --short HEAD)"
fi

echo "[INFO] Project version: ${VERSION:-unknown}"
echo "[INFO] Source commit: $COMMIT"

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
  echo "[ERROR] Cities: Skylines II was not found automatically."
  echo "Set CSII_GAMEPATH to the game installation directory, for example:"
  echo 'export CSII_GAMEPATH="$HOME/.local/share/Steam/steamapps/common/Cities Skylines II"'
  echo "Then run this script again."
  exit 1
fi

echo "[OK] Game: $FOUND"
echo "[OK] Assemblies: $FOUND/Cities2_Data/Managed"
echo "[INFO] The Unity editor and official modding toolchain are not used by this build."
echo

export CSII_GAMEPATH="$FOUND"

# Always remove previous build products so a stale DLL cannot be redeployed after source changes.
rm -rf "$ROOT/bin" "$ROOT/obj" "$ROOT/dist"

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
  echo "[ERROR] The build completed but CimRejuvenator.dll could not be located."
  exit 1
fi

DIST="$ROOT/dist/CimRejuvenator"
mkdir -p "$DIST"
cp -f "$DLL" "$DIST/CimRejuvenator.dll"

SHA256="$(sha256sum "$DIST/CimRejuvenator.dll" | awk '{print $1}')"
cat > "$DIST/BUILD_INFO.txt" <<EOF
Cim Rejuvenator
Version: ${VERSION:-unknown}
Commit: $COMMIT
SHA256: $SHA256
Build mode: direct game assemblies on Linux
EOF

echo
echo "[OK] BUILD COMPLETE"
echo "Version: ${VERSION:-unknown}"
echo "Commit: $COMMIT"
echo "SHA256: $SHA256"
echo "DLL: $DLL"
echo "Package: $DIST"

if $DEPLOY; then
  USER_DATA="${CSII_USER_DATA:-}"

  if [[ -z "$USER_DATA" ]]; then
    proton_candidates=(
      "$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II"
      "$HOME/.steam/steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II"
      "$HOME/.var/app/com.valvesoftware.Steam/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II"
    )

    for candidate in "${proton_candidates[@]}"; do
      if [[ -d "$candidate" ]]; then
        USER_DATA="$candidate"
        break
      fi
    done
  fi

  if [[ -z "$USER_DATA" || ! -d "$USER_DATA" ]]; then
    echo "[ERROR] The Cities: Skylines II Proton user-data directory was not found."
    echo "Set CSII_USER_DATA to the directory that contains Logs, Mods, and settings."
    echo "The compiled package is still available at: $DIST"
    exit 1
  fi

  MOD_DIR="$USER_DATA/Mods/CimRejuvenator"

  # Replace the local package atomically enough for a stopped game and remove stale files from old versions.
  rm -rf "$MOD_DIR"
  mkdir -p "$MOD_DIR"
  cp -f "$DIST/CimRejuvenator.dll" "$MOD_DIR/CimRejuvenator.dll"
  cp -f "$DIST/BUILD_INFO.txt" "$MOD_DIR/BUILD_INFO.txt"

  DEPLOY_SHA256="$(sha256sum "$MOD_DIR/CimRejuvenator.dll" | awk '{print $1}')"
  if [[ "$DEPLOY_SHA256" != "$SHA256" ]]; then
    echo "[ERROR] Deployed DLL checksum does not match the freshly built DLL."
    exit 1
  fi

  echo "[OK] DEPLOY COMPLETE"
  echo "Installed at: $MOD_DIR/CimRejuvenator.dll"
  echo "Verified SHA256: $DEPLOY_SHA256"

  mapfile -t DLL_COPIES < <(find "$USER_DATA" -type f -name 'CimRejuvenator.dll' -print 2>/dev/null || true)
  if (( ${#DLL_COPIES[@]} > 1 )); then
    echo
    echo "[WARNING] Multiple CimRejuvenator.dll files were found under the game user-data directory:"
    printf '  %s\n' "${DLL_COPIES[@]}"
    echo "A second copy, especially under a mod cache or another Mods folder, can make the game load an older interface."
  fi

  echo
  echo "Restart Cities: Skylines II completely before testing the new build."
  echo "After launch, the Options page should report build version ${VERSION:-unknown}."
fi
