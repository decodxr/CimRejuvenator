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

## Clone or update the repository

First clone:

```bash
cd ~
git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator
```

Existing checkout:

```bash
cd ~/CimRejuvenator
git pull
```

Confirm the source version before building:

```bash
grep '<Version>' CimRejuvenator.csproj
```

For the current release this should show `0.4.0`.

## Build and deploy

Make the script executable once:

```bash
chmod +x build-no-unity-linux.sh
```

Close Cities: Skylines II completely, then run:

```bash
./build-no-unity-linux.sh --deploy
```

The script now:

1. prints the project version and Git commit;
2. removes old `bin`, `obj`, and `dist` directories;
3. builds a fresh DLL against the installed game assemblies;
4. writes `dist/CimRejuvenator/BUILD_INFO.txt`;
5. replaces the local Proton mod directory;
6. verifies the deployed DLL SHA-256 checksum;
7. searches the game user-data directory for duplicate `CimRejuvenator.dll` files.

A successful deployment ends with `DEPLOY COMPLETE` and a verified SHA-256 value.

The default local install path is:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Mods/CimRejuvenator/
```

It should contain:

```text
CimRejuvenator.dll
BUILD_INFO.txt
```

### Custom game path

If Cities: Skylines II is stored in another Steam Library:

```bash
export CSII_GAMEPATH="/path/to/steamapps/common/Cities Skylines II"
./build-no-unity-linux.sh --deploy
```

The game path is valid when this file exists:

```text
$CSII_GAMEPATH/Cities2_Data/Managed/Game.dll
```

### Custom Proton user-data path

Set `CSII_USER_DATA` to the directory containing the game's `Logs`, settings, and local `Mods` directory:

```bash
export CSII_USER_DATA="/path/to/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II"
./build-no-unity-linux.sh --deploy
```

Do not install local development DLLs under `.cache/Mods/pdx_mods`; that directory is managed by Paradox Mods.

## If the Options page still looks old

Version 0.4.0 adds a static row in **General** named:

```text
Loaded build version
```

It must show:

```text
0.4.0
```

If that row does not exist, the currently displayed Options page did not come from the v0.4.0 settings class.

Check the deployed build metadata:

```bash
cat "$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Mods/CimRejuvenator/BUILD_INFO.txt"
```

Find every DLL copy under the user-data directory:

```bash
find "$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II" \
  -type f -name 'CimRejuvenator.dll' -print
```

Then check what the game actually loaded:

```bash
grep -RniE "Loaded CimRejuvenator|Loading Cim Rejuvenator|Loaded from" \
"$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs" \
| tail -100
```

For v0.4.0, the mod log should contain:

```text
Loading Cim Rejuvenator v0.4.0
Registered population management, flow, trend, birth-rate, and immigration systems.
```

If an old process is still running, close the game and verify it is gone before rebuilding:

```bash
pgrep -af 'Cities2|Cities.*Skylines'
```

## First v0.4.0 test

Start with the controllers disabled except rejuvenation:

```text
Enable Cim Rejuvenator:             Yes
Enable rejuvenation:                Yes
Rejuvenation chance:                80%
Maximum rejuvenations/day:          20,000
Maximum rejuvenations/sweep:        5,000
Population sweeps/day:              64

Demographic balancer:               No
Population trend controller:        No
Immigration control:                No
Birth control:                      No
```

Run the simulation and verify that **Statistics** reports the established resident population and age groups.

Then enable one controller at a time.

## Demographic balancing

A reasonable starting target is:

```text
Child:      15
Teen:       10
Adult:      60
Elderly:    15
```

The four values are relative weights and are normalized automatically.

Start with a modest conversion limit:

```text
Maximum age conversions/sweep: 2,000-5,000
Protect employed Adults:       Yes
```

The balancer skips residents currently travelling or enrolled as students.

## Population trend control

The new **Population Trend** group controls the net resident trend.

A stable-population test:

```text
Enable population trend controller: Yes
Target net population change/day:   0
Response strength:                  50%
Trend deadband:                     500
Use immigration:                    Yes
Use births:                         Yes
Maximum automatic birth rate:       250%
Allow forced outflow:               No
```

A growth target:

```text
Target net population change/day: +2,000
```

A decline target:

```text
Target net population change/day: -1,000
```

For negative targets, immigration and births can be suppressed. Actual forced resident removal only occurs when **Allow forced outflow for negative targets** is enabled. Keep it off until the positive/neutral controller has been tested successfully on the save.

The Statistics section shows:

- configured trend target;
- actual population change from the latest complete simulation day;
- smoothed population trend;
- trend-selected immigration intensity;
- trend-selected birth-rate multiplier;
- forced-outflow counters;
- controller status.

The controller needs at least one complete simulation-day transition after activation to establish a useful trend sample.

## Birth-controller compatibility

Do not run another fertility/birth-rate controller at the same time as Cim Rejuvenator's birth control or the trend controller's birth channel. Mods that write the same `CitizenParametersData` fields can overwrite each other depending on update order.

## Logs

The default Proton log directory is usually:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs
```

Check the main systems:

```bash
grep -RniE "CimRejuvenator|PopulationManagementSystem|PopulationFlowSystem|PopulationTrendSystem|BirthRateControlSystem|ImmigrationControlSystem|Exception|ERROR" \
"$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs" \
| tail -300
```

## Build failures

Save a complete build log with:

```bash
./build-no-unity-linux.sh > build-error.txt 2>&1
```

A `CSxxxx` compiler error usually means a game API field, namespace, or type differs from the version targeted by the current source. The direct build uses the installed game's assemblies, so compatibility differences appear immediately at compile time.

## Updating later

With the game closed:

```bash
cd ~/CimRejuvenator
git pull
./build-no-unity-linux.sh --deploy
```

Restart the game after deployment and confirm the **Loaded build version** row before testing new features.
