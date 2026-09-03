# Cim Rejuvenator

Cim Rejuvenator is a population-management code mod for **Cities: Skylines II**. It provides configurable rejuvenation, demographic balancing, immigration control, birth-rate control, incoming age shaping, population limits, direct or adaptive population-trend control, and live statistics.

> **Experimental release:** back up important saves before enabling large-scale life-stage conversion, direct population compensation, or forced population outflow. Version 0.5.0 can change citizen life stages, population parameters, and household inflow at runtime.

## Features

### Rejuvenation

- Rejuvenation chance from 0% to 100%.
- Configurable Adult age after rejuvenation.
- Optional minimum-health restoration.
- Up to **250,000 rejuvenations per simulation day**.
- Up to **100,000 rejuvenations per population sweep**.
- Optional minimum Elderly population share.
- Manual `REJUVENATE NOW` action.

### Demographic balancer

The established resident population can be moved gradually toward configurable Child, Teen, Adult, and Elderly target weights. The weights are normalized automatically and do not need to total 100. The default profile is `15 / 10 / 60 / 15`.

Additional controls include a maximum life-stage conversion count per sweep, worker protection, active-trip and enrolled-student protection, and a manual `BALANCE NOW` action.

### Population trend controller

The target can range from **-100,000 to +100,000 established residents per simulation day**.

```text
+5,000/day   Target strong growth
+1,000/day   Target moderate growth
0/day        Target a stable population
-1,000/day   Target gradual decline
```

Two control modes are available.

#### Adaptive mode

Adaptive mode is the less invasive controller. It measures the resident change once per simulation day, smooths the result, and adjusts controllable rates over time:

- resident immigration intensity;
- birth-rate multiplier;
- optional household outflow for negative targets.

This mode steers the simulation but cannot guarantee that deaths or other losses will be replaced immediately.

#### Direct trend compensation

Direct mode is intended for cities that keep losing population even with immigration and births at high settings.

At the end of each complete simulation day, direct mode compares the **actual established-resident change** with the selected target. When the city is below target, it schedules normal vanilla resident households to compensate the measured shortfall.

Example:

```text
Target:             0/day
Actual last day: -5,000/day
Correction:      +5,000 residents at 100% correction strength
```

Another example:

```text
Target:          +2,000/day
Actual last day: -3,000/day
Correction:      +5,000 residents at 100% correction strength
```

Direct mode uses normal household prefabs, outside connections, `PrefabRef`, `CurrentBuilding`, and the game's household initialization pipeline. It does **not** create standalone citizen entities manually. The displayed direct-resident count is therefore an estimate based on the selected household prefab composition; the game still performs household initialization and moving-in normally.

Direct controls include:

- correction strength from 10% to 100%;
- maximum direct correction up to **250,000 estimated residents per simulation day**;
- optional immigration and birth-rate assist channels;
- optional population ceiling;
- incoming age shaping for directly injected households;
- optional forced outflow when growth is above a negative or lower target.

At 100% correction strength, the controller attempts to compensate the full measured shortfall, subject to the direct daily cap, deadband, population ceiling, available household prefabs, and outside connections.

Forced outflow is **disabled by default**. If the city grows faster than the selected target while forced outflow is disabled, direct mode only throttles the assist channels rather than deleting residents.

Large direct corrections can create immediate housing, traffic, employment, education, and service demand. Start with a moderate daily cap on an important save.

### Immigration control

- Immigration intensity from 0% to 100%.
- Optional soft daily cap for new residents.
- Optional resident population ceiling.
- Incoming age-mix shaping with separate Child, Teen, Adult, and Elderly weights.
- Live immigration status.

Manual immigration control gates the game's resident `HouseholdSpawnSystem`. Work already in progress can finish slightly above a selected soft cap.

When trend immigration control is active, the trend controller supplies the effective immigration intensity. In direct mode, immigration can be used as an assist channel while direct household compensation handles measured shortfalls.

### Birth control

- Birth-rate multiplier from 0% to 500%.
- Optional soft daily birth cap.
- Optional pause when the Child population reaches the configured demographic target.
- Live display of the applied birth-rate multiplier.

The controller scales `CitizenParametersData.m_BaseBirthRate` and `m_AdultFemaleBirthRateBonus`. Original values are captured and restored when control is released.

### Population statistics

The Options panel reports the loaded DLL version, resident census by life stage, rejuvenation and demographic conversion counters, detected births and incoming residents, birth and immigration controller state, trend target and measured change, effective trend rates, direct correction request, direct residents and households scheduled, forced outflow, and population sweep counters.

## Recommended profiles

### Severe population-loss recovery

For a city that is actively losing residents despite normal immigration:

```text
Population trend control:                On
Direct trend compensation:               On
Target net population change/day:        0 to +2,000
Direct correction strength:              100%
Maximum direct residents/day:            25,000 to 50,000
Trend deadband:                          0 to 100
Use immigration for trend control:       On
Use births for trend control:            On
Maximum automatic birth rate:            250-300%
Forced outflow:                          Off
```

If the loss wave is extremely large, raise the direct daily cap gradually rather than immediately selecting 250,000.

### Elderly-wave recovery

```text
Rejuvenation enabled:              Yes
Rejuvenation chance:               80%
Age after rejuvenation:            40
Restore minimum health:            Yes
Maximum rejuvenations/day:         20,000
Maximum rejuvenations/sweep:       5,000
Minimum Elderly protection:        Off during recovery

Demographic balancer:              Off initially
Population trend controller:       Off initially
Birth control:                     Off initially
Immigration control:               Off initially
Population sweeps/day:             64
```

After the city stabilizes, a useful demographic target is:

```text
Child:      15
Teen:       10
Adult:      60
Elderly:    15
```

Enable the demographic balancer with a modest conversion limit, such as 2,000-5,000 per sweep, before trying higher values.

### Adaptive stable-population control

```text
Population trend control:          On
Direct trend compensation:         Off
Target net change/day:             0
Response strength:                 40-50%
Deadband:                          500
Use immigration:                   On
Use births:                        On
Maximum automatic birth rate:      200-300%
Forced outflow:                    Off
```

## Build

The project supports two build paths:

- Directly against the assemblies installed with Cities: Skylines II. This does **not** require Unity activation and works on Windows or Linux.
- The official Cities: Skylines II modding toolchain when `CSII_TOOLPATH` is available.

### Linux / Proton

```bash
git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator
chmod +x build-no-unity-linux.sh
./build-no-unity-linux.sh --deploy
```

For later updates:

```bash
cd ~/CimRejuvenator
git pull --ff-only
./build-no-unity-linux.sh --deploy
```

The Linux build script performs a clean build, writes `BUILD_INFO.txt`, verifies the deployed DLL checksum, and warns if multiple `CimRejuvenator.dll` files are found under the Cities: Skylines II user-data directory.

See [TUTORIAL-LINUX.md](TUTORIAL-LINUX.md) for game-path and Proton-path overrides.

### Windows without Unity

```powershell
git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator
Set-ExecutionPolicy -Scope Process Bypass
.\check-environment.ps1
.\build-no-unity.ps1
```

See [TUTORIAL-WINDOWS.md](TUTORIAL-WINDOWS.md) for alternate Steam Library paths and troubleshooting.

## Verifying that the current DLL is loaded

Under **Options -> Cim Rejuvenator -> General**, **Loaded build version** should report:

```text
0.5.0
```

The deployed package also contains:

```text
Mods/CimRejuvenator/BUILD_INFO.txt
```

On Linux, locate every local DLL copy with:

```bash
find "$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II" \
  -type f -name 'CimRejuvenator.dll' -print
```

Fully close Cities: Skylines II before replacing a code-mod DLL and restart the game after deployment.

## Local Linux installation

The default Proton user-data directory is usually:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II
```

A local build should end up at:

```text
.../Cities Skylines II/Mods/CimRejuvenator/CimRejuvenator.dll
```

Do not install local development DLLs inside `.cache/Mods/pdx_mods`; that directory is managed by Paradox Mods.

## Compatibility

Cim Rejuvenator changes population-related systems and parameters. Mods that modify the same areas can override each other depending on update order.

Avoid enabling two birth/fertility controllers at the same time. Any mod that writes `CitizenParametersData.m_BaseBirthRate` or `m_AdultFemaleBirthRateBonus` overlaps with Cim Rejuvenator's birth controller and trend birth channel.

Immigration-control mods that enable or disable `HouseholdSpawnSystem` overlap with this mod's immigration controller and trend immigration channel.

Direct trend compensation adds ordinary household entities through the vanilla household initialization path. Mods that replace household spawning or initialization may therefore alter its results.

The demographic balancer preserves household identity and the citizen entity, but changing life stage can still affect employment, school demand, transport patterns, and household behaviour.

## Implementation notes

Cities: Skylines II represents life stage through `Citizen.GetAge()` / `Citizen.SetAge()`. `m_BirthDay` stores a simulation-day number, so life-stage conversions update both the age bits and birth day. The current age thresholds used by the game are:

```text
Child -> Teen:       21 days
Teen -> Adult:       36 days
Adult -> Elderly:    84 days
```

Population management uses living residents in moved-in households and excludes tourists, commuters, and households already moving away.

Incoming-resident tracking establishes a baseline when the system starts, then detects new resident entities. Newborn detection uses newly created Child residents whose calculated age is zero days. These are mod-side flow counters and may not exactly match every vanilla UI counter at the same frame.

Direct trend compensation intentionally works at the household level. The controller schedules a vanilla household archetype at an outside connection, after which normal household initialization creates and manages its citizens.

## Logs

On Linux / Proton, logs are normally located under:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs
```

Useful command:

```bash
grep -RniE "CimRejuvenator|PopulationManagementSystem|PopulationTrendSystem|Exception|ERROR" \
"$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs" \
| tail -250
```

## License

Cim Rejuvenator is **source-available for noncommercial use** under the [Cim Rejuvenator Noncommercial Attribution License 1.0](LICENSE).

You may use, study, modify, fork, and redistribute the project for noncommercial purposes. Redistribution and derivative works must keep the license, credit **Cim Rejuvenator by decodxr**, link to the original repository when links are supported, and clearly identify modifications.

Selling the mod or a derivative, charging for access or downloads, placing it behind a paid tier, or otherwise monetizing distribution of code derived from this project is not permitted without prior written permission from the copyright holder.

Ordinary monetized videos, livestreams, reviews, screenshots, and tutorials that merely feature the mod are allowed as long as access to the software itself remains free.

Because commercial use is restricted, this project should be described as **source-available**, not OSI Open Source.

### Earlier MIT copies

The license change is not retroactive. Copies that were already distributed under MIT remain under the MIT license that accompanied those copies. Current and future copies distributed with the new `LICENSE` file use the noncommercial attribution terms.
