# Cim Rejuvenator

Cim Rejuvenator is a population-management code mod for **Cities: Skylines II**. It started as a tool for recovering cities from extreme elderly population waves and now includes configurable rejuvenation, demographic balancing, immigration throttling, birth-rate control, incoming age shaping, population limits, and live statistics.

> **Experimental release:** make a backup of important saves before enabling population conversion features. Version 0.3.0 changes citizen life stages and simulation parameters at runtime.

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

The resident population can be moved gradually toward four configurable target weights:

- Child
- Teen
- Adult
- Elderly

The weights are normalized automatically, so they do not need to total 100. A default balanced profile is `15 / 10 / 60 / 15`.

Additional controls include:

- Maximum life-stage conversions per sweep.
- Worker protection to avoid converting employed Adults into non-working life stages.
- Manual `BALANCE NOW` action.

### Immigration control

- Immigration intensity from 0% to 100%.
- Optional soft daily cap for new resident citizens.
- Optional resident population ceiling.
- Incoming age-mix shaping with separate Child, Teen, Adult, and Elderly weights.
- Live immigration status in the statistics section.

Immigration control gates the game's resident `HouseholdSpawnSystem`. Daily limits are soft limits: a household that is already being created can cause the detected citizen count to finish slightly above the selected cap.

### Birth control

- Birth-rate multiplier from 0% to 500%.
- Optional soft daily birth cap.
- Optional automatic pause when the Child population reaches the configured demographic target.
- Live display of the currently applied birth-rate multiplier.

The controller scales the game's `CitizenParametersData.m_BaseBirthRate` and `m_AdultFemaleBirthRateBonus`. The original values are captured and restored when birth control is disabled or the system is destroyed.

### Population statistics

The Options panel reports:

- Resident population.
- Child, Teen, Adult, and Elderly counts and percentages.
- Rejuvenations for the latest sweep, current day, and session.
- Demographic conversions for the latest sweep and session.
- Detected births for the current day and session.
- Detected new residents for the current day and session.
- Applied birth-rate multiplier.
- Immigration controller status.
- Population sweep count and last scanned simulation day.

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

## Aggressive recovery profile

For a one-time recovery test on a backup save:

```text
Rejuvenation chance:               100%
Maximum rejuvenations/day:         100,000
Maximum rejuvenations/sweep:       50,000
Population sweeps/day:             64
```

The absolute selectable limits are 250,000 per day and 100,000 per sweep. Large conversions can create immediate labour-market, education, traffic, and service-demand changes.

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

In particular, avoid enabling two different birth/fertility controllers at the same time. Any mod that writes `CitizenParametersData.m_BaseBirthRate` or `m_AdultFemaleBirthRateBonus` overlaps with Cim Rejuvenator's birth controller.

Immigration-control mods that enable or disable `HouseholdSpawnSystem` also overlap with this mod's immigration controller.

The demographic balancer deliberately preserves household identity and the citizen entity, but changing life stage can still affect employment, school demand, transport patterns, and household behaviour.

## Implementation notes

Cities: Skylines II represents life stage through `Citizen.GetAge()` / `Citizen.SetAge()`. `m_BirthDay` stores a simulation-day number, so life-stage conversions update both the age bits and birth day. The game currently uses these age thresholds:

```text
Child -> Teen:       21 days
Teen -> Adult:       36 days
Adult -> Elderly:    84 days
```

Incoming-resident tracking establishes a baseline when the system starts, then counts newly created resident citizen entities. Newborn detection is based on newly created Child residents whose calculated age is zero days.

## Logs

On Linux / Proton, logs are normally located under:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs
```

Useful command:

```bash
grep -RniE "CimRejuvenator|PopulationManagementSystem|Exception|ERROR" \
"$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs" \
| tail -200
```

## License

Cim Rejuvenator is **source-available for noncommercial use** under the [Cim Rejuvenator Noncommercial Attribution License 1.0](LICENSE).

You may use, study, modify, fork, and redistribute the project for noncommercial purposes. Redistribution and derivative works must keep the license, credit **Cim Rejuvenator by decodxr**, link to the original repository when links are supported, and clearly identify modifications.

Selling the mod or a derivative, charging for access or downloads, placing it behind a paid tier, or otherwise monetizing distribution of code derived from this project is not permitted without prior written permission from the copyright holder.

Ordinary monetized videos, livestreams, reviews, screenshots, and tutorials that merely feature the mod are allowed as long as access to the software itself remains free.

Because commercial use is restricted, this project should be described as **source-available**, not OSI Open Source.

### Earlier MIT copies

The license change is not retroactive. Copies that were already distributed under MIT remain under the MIT license that accompanied those copies. Current and future copies distributed with the new `LICENSE` file use the noncommercial attribution terms.
