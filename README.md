# Cim Rejuvenator

Cim Rejuvenator is a population-management code mod for **Cities: Skylines II**. It provides configurable rejuvenation, demographic balancing, immigration control, birth-rate control, incoming age shaping, population limits, population-trend control, and live statistics.

> **Experimental release:** back up important saves before enabling direct life-stage conversion or forced population outflow. Version 0.4.0 changes citizen life stages and simulation parameters at runtime.

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

The established resident population can be moved gradually toward four configurable target weights:

- Child
- Teen
- Adult
- Elderly

The weights are normalized automatically, so they do not need to total 100. The default profile is `15 / 10 / 60 / 15`.

Additional controls include:

- Maximum life-stage conversions per sweep.
- Worker protection to avoid converting employed Adults into non-working life stages.
- Active-trip and enrolled-student protection.
- Manual `BALANCE NOW` action.

### Population trend controller

Version 0.4.0 adds a feedback controller for net population movement. The target can range from **-100,000 to +100,000 residents per simulation day**.

Examples:

```text
+5,000/day   Target strong growth
+1,000/day   Target moderate growth
0/day        Target a roughly stable population
-1,000/day   Target gradual decline
```

The controller measures the resident change once per simulation day, smooths the result, and can automatically adjust:

- resident immigration intensity;
- the birth-rate multiplier;
- optional household outflow for negative targets.

Forced outflow is **disabled by default**. With it disabled, a negative target only suppresses controllable inflow and does not forcibly remove residents. When enabled, the controller can mark moved-in resident households to leave the city, subject to a configurable daily soft cap.

Other trend controls include response strength, deadband, a maximum automatic birth-rate multiplier, and a controller reset action.

### Immigration control

- Immigration intensity from 0% to 100%.
- Optional soft daily cap for new resident citizens.
- Optional resident population ceiling.
- Incoming age-mix shaping with separate Child, Teen, Adult, and Elderly weights.
- Live immigration status in the statistics section.

Immigration control gates the game's resident `HouseholdSpawnSystem`. Daily limits are soft limits: work already in progress can finish slightly above a selected cap.

When population-trend immigration control is active, the trend controller supplies the effective immigration intensity. Manual caps and the population ceiling still belong to the manual immigration controller.

### Birth control

- Birth-rate multiplier from 0% to 500%.
- Optional soft daily birth cap.
- Optional automatic pause when the Child population reaches the configured demographic target.
- Live display of the currently applied birth-rate multiplier.

The controller scales `CitizenParametersData.m_BaseBirthRate` and `m_AdultFemaleBirthRateBonus`. Original values are captured and restored when control is released.

When population-trend birth control is active, the trend controller supplies the effective birth-rate multiplier, bounded by the configured automatic maximum.

### Population statistics

The Options panel reports:

- loaded DLL version;
- established resident population;
- Child, Teen, Adult, and Elderly counts and percentages;
- rejuvenation and demographic-conversion counters;
- detected births and incoming residents;
- effective birth-rate multiplier;
- immigration-controller status;
- population-trend target, last daily change, and smoothed change;
- trend-selected immigration and birth rates;
- forced-outflow counters;
- population sweep count and last scanned simulation day.

## Recommended starting profile

For recovery from a severe elderly population wave:

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

For a stable-population experiment after the census has settled:

```text
Population trend control:          On
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
git pull
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

If the Options page still looks like an older release, first check **Loaded build version** under General. Version 0.4.0 must report:

```text
0.4.0
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

A second local or cached copy can make debugging version mismatches difficult. Fully close Cities: Skylines II before replacing a code-mod DLL and restart the game after deployment.

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

Avoid enabling two birth/fertility controllers at the same time. Any mod that writes `CitizenParametersData.m_BaseBirthRate` or `m_AdultFemaleBirthRateBonus` overlaps with Cim Rejuvenator's birth controller and the birth channel of population-trend control.

Immigration-control mods that enable or disable `HouseholdSpawnSystem` overlap with this mod's immigration controller and trend immigration channel.

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
