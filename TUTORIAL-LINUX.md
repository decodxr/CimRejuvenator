# Linux / Proton Build and Install Guide

This guide covers local development builds of Cim Rejuvenator on Linux. The direct-assembly build references the DLLs shipped with Cities: Skylines II and does not require the Unity editor or Unity license activation.

> Back up important saves before testing a new development build.

## Requirements

On Arch Linux:

```bash
sudo pacman -S git dotnet-sdk
```

Verify both tools:

```bash
git --version
dotnet --version
```

## Clone the repository

```bash
cd ~
git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator
```

For an existing checkout:

```bash
cd ~/CimRejuvenator
git pull
```

## Build

Make the build script executable once:

```bash
chmod +x build-no-unity-linux.sh
```

Build the DLL:

```bash
./build-no-unity-linux.sh
```

The output is copied to:

```text
dist/CimRejuvenator/CimRejuvenator.dll
```

### Custom game path

The script checks common Steam locations automatically. For a custom Steam Library, set `CSII_GAMEPATH` to the directory containing `Cities2_Data`:

```bash
export CSII_GAMEPATH="/path/to/steamapps/common/Cities Skylines II"
./build-no-unity-linux.sh
```

The path is valid when this file exists:

```text
$CSII_GAMEPATH/Cities2_Data/Managed/Game.dll
```

## Build and deploy to Proton

Close Cities: Skylines II before replacing a loaded code-mod DLL.

```bash
./build-no-unity-linux.sh --deploy
```

The script checks common Proton prefixes for Steam AppID `949230` and installs the DLL under:

```text
.../Cities Skylines II/Mods/CimRejuvenator/CimRejuvenator.dll
```

### Custom Proton user-data path

If the prefix is in a custom location, set `CSII_USER_DATA` to the directory containing the game's `Logs`, settings, and local `Mods` directory:

```bash
export CSII_USER_DATA="/path/to/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II"
./build-no-unity-linux.sh --deploy
```

Do not install local development DLLs under `.cache/Mods/pdx_mods`; that directory is managed by Paradox Mods.

## First v0.3.0 test

Start with population control features disabled except rejuvenation. Confirm the population census updates before enabling the new controllers.

Suggested first test:

```text
Enable Cim Rejuvenator:             Yes
Enable rejuvenation:                Yes
Rejuvenation chance:                80%
Maximum rejuvenations/day:          20,000
Maximum rejuvenations/sweep:        5,000
Population sweeps/day:              64

Demographic balancer:               No
Immigration control:                No
Birth control:                      No
```

After the census is updating, enable one new controller at a time.

A reasonable demographic target is:

```text
Child:      15
Teen:       10
Adult:      60
Elderly:    15
```

The four target values are relative weights and are normalized automatically.

## Logs

The default Proton log directory is usually:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs
```

Check Cim Rejuvenator messages:

```bash
grep -RniE "CimRejuvenator|PopulationManagementSystem|PopulationFlowSystem|BirthRateControlSystem|ImmigrationControlSystem|Exception|ERROR" \
"$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs" \
| tail -250
```

The mod-specific log is normally:

```text
.../Cities Skylines II/Logs/CimRejuvenator.log
```

## Build failures

Save a complete build log with:

```bash
./build-no-unity-linux.sh > build-error.txt 2>&1
```

A `CSxxxx` compiler error usually means a game API field, namespace, or type differs from the version targeted by the current source. The direct build deliberately uses the installed game's assemblies, so these errors expose compatibility changes immediately.

## Updating a local installation

With the game closed:

```bash
cd ~/CimRejuvenator
git pull
./build-no-unity-linux.sh --deploy
```

Restart the game after deployment.
