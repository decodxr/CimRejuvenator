# Cim Rejuvenator

Cim Rejuvenator is a population-management code mod for **Cities: Skylines II**. It combines rejuvenation, demographic balancing, immigration control, birth-rate control, incoming age shaping, population limits, adaptive trend control, and a direct growth-lock system for cities suffering severe population collapse.

> **Experimental release:** back up important saves before using large-scale life-stage conversion, direct household injection, or forced population outflow. Version 0.6.0 changes citizen life stages and population simulation parameters at runtime.

## Highlights in 0.6.0

- Continuous direct growth lock instead of one correction per simulation day.
- A zero target can be used as a **no-decline lock**.
- Positive targets create a protected upward population trajectory.
- Direct corrections react throughout the day and compensate sudden death-wave losses.
- Separate daily and per-check direct-injection safety limits.
- Emergency growth preset for severe population collapse.
- Population target range increased to **-500,000 through +500,000 residents/day**.
- Direct daily capacity increased to **1,000,000 residents/day**.
- Birth-rate controls increased to **1,000%**.
- New diagnostics for protected population floor, shortfall, and pending direct residents.
- Portuguese localization is selected automatically when the game runs in a Portuguese locale. Unsupported locales receive the complete English fallback.

## Population trend control

### Adaptive mode

Adaptive mode steers the normal simulation by changing immigration intensity and birth-rate multipliers. It is less invasive, but it cannot guarantee growth if vanilla demand, housing, or a death wave overwhelms those channels.

### Continuous direct growth lock

Direct mode is intended for population emergencies.

For a target of `0/day`, the controller keeps a high-water population floor. If the established resident count falls below that floor, the mod schedules normal household entities from outside connections to replace the shortfall.

For a positive target, the floor also moves upward during the simulation day. For example:

```text
Target: +5,000/day
Current established population falls by 8,000
Growth lock detects the shortfall
The controller schedules enough resident households to recover the loss
The protected floor continues moving toward +5,000/day
```

Direct mode runs multiple checks per day instead of waiting for a complete day before reacting. Scheduled households still pass through the game's normal household initialization and move-in pipeline, so the vanilla population indicator can lag behind a correction for a short time.

The controller intentionally credits only part of scheduled-but-not-yet-established residents. This makes it retry aggressively during a death wave instead of assuming every pending household will arrive immediately.

### Emergency growth preset

The Options page contains `APPLY EMERGENCY GROWTH PRESET`. It configures:

```text
Population trend control:             On
Continuous direct growth lock:        On
Target net population change/day:     +5,000
Direct correction strength:           100%
Maximum direct residents/day:         250,000
Maximum direct residents/check:       50,000
Trend deadband:                        0
Immigration assist:                    On
Birth assist:                          On
Maximum automatic birth rate:         500%
Forced outflow:                        Off
Manual immigration controller:        Off
Manual birth controller:              Off
```

For a city that must never shrink, use direct mode with a target of `0/day` or any positive value and leave forced outflow disabled.

## Rejuvenation

- Rejuvenation chance from 0% to 100%.
- Configurable Adult age after rejuvenation.
- Optional minimum-health restoration.
- Up to **250,000 rejuvenations per simulation day**.
- Up to **100,000 rejuvenations per population sweep**.
- Optional minimum Elderly share.
- Manual `REJUVENATE NOW` action.

## Demographic balancer

The established resident population can be moved toward configurable Child, Teen, Adult, and Elderly target weights. Weights are normalized automatically and do not need to total 100.

The default demographic profile is:

```text
Child:    15
Teen:     10
Adult:    60
Elderly:  15
```

Worker, active-trip, and enrolled-student protections reduce disruptive life-stage conversion.

## Immigration control

- Manual immigration intensity from 0% to 100%.
- Optional daily incoming-resident cap up to 1,000,000.
- Optional resident population ceiling up to 5,000,000.
- Incoming age shaping with separate Child, Teen, Adult, and Elderly weights.

The direct growth lock can inject resident households without relying on positive residential demand. Manual immigration caps and ceilings only apply when the manual immigration controller is enabled.

## Birth control

- Birth-rate multiplier from 0% to 1,000%.
- Optional daily birth cap.
- Optional pause when the Child demographic target is reached.
- Trend control can use births as an assist channel.

Cim Rejuvenator scales `CitizenParametersData.m_BaseBirthRate` and `m_AdultFemaleBirthRateBonus` while birth control is active and restores captured baseline values when control is released.

## Localization

The mod registers localization against the game's supported locales on startup.

- Portuguese game locale: Portuguese UI.
- Other currently unsupported languages: complete English fallback.

The repository source, comments, documentation, and build tooling remain in English. Translation strings are kept in dedicated locale source files.

## Statistics and diagnostics

The Options page reports:

- loaded DLL version;
- established resident population;
- Child, Teen, Adult, and Elderly counts;
- rejuvenation and demographic-conversion counters;
- detected births and incoming residents;
- effective immigration and birth rates;
- trend target and measured daily change;
- protected growth floor;
- current growth-lock shortfall;
- pending direct residents;
- direct residents and households scheduled;
- forced-outflow counters;
- controller status.

## Build on Linux / Proton

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

The direct build uses the managed assemblies from the installed game and does not require Unity editor activation.

The default Proton deployment target is:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Mods/CimRejuvenator
```

## Build on Windows without Unity

```powershell
git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator
Set-ExecutionPolicy -Scope Process Bypass
.\check-environment.ps1
.\build-no-unity.ps1
```

## Compatibility

Population mods can overlap with Cim Rejuvenator when they control the same simulation systems.

- Avoid running another fertility mod while Cim Rejuvenator birth control or trend birth assist is active.
- Immigration mods that directly enable or disable `HouseholdSpawnSystem` can conflict with immigration control.
- Direct growth lock schedules normal household entities and can create substantial housing, traffic, education, and employment demand when configured aggressively.

## Logs

Linux / Proton logs are normally under:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs
```

Useful command:

```bash
grep -RniE "CimRejuvenator|PopulationTrendSystem|Exception|ERROR" \
"$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs" \
| tail -250
```

## License

Cim Rejuvenator is **source-available for noncommercial use** under the [Cim Rejuvenator Noncommercial Attribution License 1.0](LICENSE).

You may use, study, modify, fork, and redistribute the current project for noncommercial purposes. Redistribution and derivative works must preserve the license and notice, credit **Cim Rejuvenator by decodxr**, link to the original repository when links are supported, and clearly identify modifications.

Selling the software or a derivative, charging for downloads or access, placing it behind a paid tier, or otherwise monetizing distribution of code derived from the current project is not permitted without prior written permission from the copyright holder.

Ordinary monetized videos, livestreams, reviews, screenshots, and tutorials that merely feature the mod are allowed as long as access to the software itself remains free.

Earlier copies that were already distributed under MIT remain governed by the MIT license that accompanied those copies.
