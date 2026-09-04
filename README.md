# Cim Rejuvenator

Cim Rejuvenator is a population-management code mod for **Cities: Skylines II**. It combines a hard resident death lock, rejuvenation, demographic balancing, immigration and birth controls, incoming age shaping, population limits, adaptive trend control, and direct population recovery.

> **Experimental:** back up important saves before testing aggressive population controls. The mod changes citizen state and simulation systems at runtime.

## Highlights in 0.7.0

- Hard death lock for established residents.
- Normal vanilla old-age and sickness/injury deaths are stopped by suspending `DeathCheckSystem` while the mod is enabled.
- Additional guards rescue resident death states created by sickness, health-problem, disaster, and event paths.
- A final pre-removal guard revives any remaining resident corpse before vanilla citizen removal.
- Residents may still leave the city normally; moving away is not blocked.
- Existing population trend controls remain available for deliberate growth and for replacing move-outs.
- Portuguese UI is selected automatically for Portuguese game locales; unsupported languages receive English fallback strings.

## Death lock

Version 0.7.0 changes the population-safety model. Instead of trying to replace every person after a death, the mod prevents established residents from remaining dead long enough to be removed from the city.

The protection has four layers:

1. `DeathCheckSystem` is disabled while the Cim Rejuvenator master switch is enabled. This blocks the normal old-age and sickness/injury death rolls.
2. A guard after `SicknessCheckSystem` clears fatal health states created by sickness checks.
3. A guard after `AddHealthProblemSystem` clears disaster and event death states.
4. A guard after `HealthProblemSystem`, plus a final guard before `HouseholdAndCitizenRemoveSystem`, clears any remaining resident death state before removal.

When a protected resident is rescued, the same citizen entity and household membership are preserved. Fatal, transport, danger, sickness, and injury flags are cleared and health is restored to 100.

The lock applies to **established residents in moved-in households**. Tourists and commuters are not protected. Households marked to move away are not prevented from leaving, so population can still fall because residents genuinely move out.

Some event systems can emit a casualty statistic at the moment they attempt a fatal event, before the protection guard clears the death state. For that reason, the most important validation is the resident population and whether citizens are actually removed, not a same-tick event counter.

Turning off the Cim Rejuvenator master switch restores the previous enabled state of the vanilla `DeathCheckSystem`.

## Rejuvenation

- Rejuvenation chance from 0% to 100%.
- Configurable Adult age after rejuvenation.
- Optional health restoration.
- Up to **250,000 rejuvenations per simulation day**.
- Up to **100,000 rejuvenations per population sweep**.
- Optional minimum Elderly share.
- Manual `REJUVENATE NOW` action.

## Demographic balancer

The established resident population can be moved toward configurable Child, Teen, Adult, and Elderly target weights. Weights are normalized automatically and do not need to total 100.

Default target profile:

```text
Child:    15
Teen:     10
Adult:    60
Elderly:  15
```

Worker, active-trip, and enrolled-student protections reduce disruptive life-stage conversion.

## Population trend control

### Adaptive mode

Adaptive mode keeps the vanilla population flow and steers immigration and birth rates toward the selected net population target. It is the less invasive mode.

### Continuous direct growth lock

Direct mode monitors the vanilla city population component throughout the simulation day and maintains a protected population floor.

- `0/day` targets no decline.
- Positive values move the protected floor upward.
- Negative values can suppress inflow and optionally force household outflow.

When population falls below the protected trajectory, direct mode can schedule normal resident household prefabs through outside connections instead of waiting for residential demand alone.

The target range is **-500,000 to +500,000 residents/day**. Direct injection can be capped separately per check and per simulation day.

With the 0.7.0 death lock active, direct trend control is mainly useful for deliberate growth and for compensating residents who actually move away rather than for replacing deaths.

## Immigration control

- Manual immigration intensity from 0% to 100%.
- Optional incoming-resident cap up to 1,000,000/day.
- Optional resident population ceiling up to 5,000,000.
- Incoming age shaping with separate Child, Teen, Adult, and Elderly weights.

## Birth control

- Birth-rate multiplier from 0% to 1,000%.
- Optional daily birth cap.
- Optional pause at the Child demographic target.
- Population trend control can use births as an assist channel.

Cim Rejuvenator scales `CitizenParametersData.m_BaseBirthRate` and `m_AdultFemaleBirthRateBonus` while birth control is active and restores captured baseline values when control is released.

## Localization

Localization follows the game locale at startup.

- Portuguese locale: Portuguese UI.
- Other currently unsupported locales: English fallback.

Repository source code, comments, documentation, and build tooling remain in English. Translation strings live in dedicated locale source files.

## Build on Linux / Proton

Initial clone:

```bash
git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator
chmod +x build-no-unity-linux.sh
./build-no-unity-linux.sh --deploy
```

Update an existing clone:

```bash
cd ~/CimRejuvenator
git pull --ff-only
./build-no-unity-linux.sh --deploy
```

The direct build uses managed assemblies from the installed game and does not require Unity editor activation.

Default Proton deployment target:

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

Population mods can overlap when they control the same vanilla systems.

- Birth/fertility mods can conflict with Cim Rejuvenator birth control.
- Immigration mods that enable or disable `HouseholdSpawnSystem` can conflict with immigration control.
- Mods that replace `DeathCheckSystem`, `SicknessCheckSystem`, `HealthProblemSystem`, `AddHealthProblemSystem`, or resident-removal behavior can conflict with the 0.7.0 death lock.
- Aggressive direct household injection can create substantial housing, traffic, education, employment, and service demand.

## Logs

Linux / Proton logs are normally under:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs
```

Useful command:

```bash
grep -RniE "CimRejuvenator|Death lock|PopulationTrendSystem|Exception|ERROR" \
"$HOME/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Logs" \
| tail -300
```

## License

Cim Rejuvenator is **source-available for noncommercial use** under the [Cim Rejuvenator Noncommercial Attribution License 1.0](LICENSE).

You may use, study, modify, fork, and redistribute the current project for noncommercial purposes. Redistribution and derivative works must preserve the license and notice, credit **Cim Rejuvenator by decodxr**, link to the original repository when links are supported, and clearly identify modifications.

Selling the software or a derivative, charging for downloads or access, placing it behind a paid tier, or otherwise monetizing distribution of code derived from the current project is not permitted without prior written permission from the copyright holder.

Ordinary monetized videos, livestreams, reviews, screenshots, and tutorials that merely feature the mod are allowed as long as access to the software itself remains free.

Earlier copies already distributed under MIT remain governed by the MIT license that accompanied those copies.
