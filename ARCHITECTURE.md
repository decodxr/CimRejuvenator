# Architecture

Cim Rejuvenator is split into five simulation systems so population conversion, flow tracking, trend control, birth-rate control, and immigration gating remain separated.

## PopulationManagementSystem

Runs a configurable number of full established-resident sweeps per simulation day.

Responsibilities:

- Build the resident census by `CitizenAge`.
- Require a living citizen in a moved-in resident household.
- Exclude tourists, commuters, moving-away households, deleted entities, temporary entities, and dead citizens from managed demographic counts.
- Rejuvenate eligible Elderly residents.
- Apply optional minimum-Elderly-share protection.
- Move the population toward normalized Child / Teen / Adult / Elderly target weights.
- Enforce per-sweep conversion limits.
- Preserve employed Adults when worker protection is enabled.
- Skip demographic conversion while a citizen is in an active trip or enrolled as a student.

Life-stage conversions use `Citizen.SetAge()` and update `Citizen.m_BirthDay` to keep the age bits and calculated age consistent. Child and Teen conversions also clear the partner-seeking flag.

## PopulationFlowSystem

Maintains a session-local set of observed resident entity IDs.

The first update establishes a baseline. Later newly detected living residents in moved-in households are classified as newborns or incoming residents. Incoming age shaping can apply to manual immigration and to households introduced by direct growth lock.

The scanner periodically prunes entity IDs that no longer belong to active resident entities. Daily birth and immigration limits are soft caps because detection happens after entities are created.

## PopulationTrendSystem

The system supports adaptive feedback control and continuous direct growth lock.

### Adaptive mode

Adaptive mode records the established-resident change across simulation-day boundaries, maintains an exponential moving average, and adjusts enabled control channels toward the selected target:

- effective immigration intensity;
- effective birth-rate multiplier;
- optional forced household outflow for negative targets.

It keeps normal demand-driven population flow and is therefore the less invasive mode, but a large death wave can overwhelm it.

### Continuous direct growth lock

For zero and positive targets, direct mode no longer waits for a complete day before correcting population loss.

The system maintains:

- a high-water established-resident count;
- a day-start population anchor;
- a target trajectory that advances during the day for positive targets;
- a protected population floor equal to the greater of the high-water count and the target trajectory;
- an estimate of directly scheduled residents that have not yet appeared in the established-resident census.

A target of `0/day` is therefore a no-decline lock: the protected floor cannot move downward while direct mode remains active. A positive target raises that protected floor throughout the day.

At each direct check, the controller compares the current established population plus partial credit for pending direct residents against the protected floor. When the city is short, it schedules additional resident households subject to correction strength, a per-check limit, and a daily limit.

Only part of pending direct population is trusted when calculating the next shortfall. Pending population is also partially retried at day boundaries. This intentionally biases emergency recovery toward over-correction rather than letting a death wave continue while the controller waits for slow move-in completion.

### Direct household injection path

Direct mode does not construct standalone citizen entities. It mirrors the vanilla resident household entry path:

1. query normal household prefabs with `ArchetypeData` and `HouseholdData`, excluding `DynamicHousehold`;
2. select household prefabs using their normal `m_Weight` and estimated household size;
3. create the household from the prefab archetype through `EndFrameBarrier`;
4. set `PrefabRef`;
5. add `CurrentBuilding` pointing to a valid non-utility outside connection;
6. leave citizen creation, household initialization, property seeking, and move-in completion to the normal game pipeline.

Direct resident counters are scheduled estimates until those households become established residents.

For negative direct targets, inflow can be suppressed and forced household outflow remains separately opt-in. Positive and zero direct targets do not require forced removal.

## BirthRateControlSystem

Captures the original `CitizenParametersData` values before applying birth control.

The effective birth multiplier scales:

- `m_BaseBirthRate`
- `m_AdultFemaleBirthRateBonus`

`m_StudentBirthRateAdjust` is left unchanged because it is a separate multiplier in the game's birth calculation.

The effective multiplier can come from either the manual birth controller or `PopulationTrendSystem`. Original parameter values are restored when no controller needs birth-rate ownership or when the system is destroyed.

## ImmigrationControlSystem

Controls `HouseholdSpawnSystem.Enabled` for ordinary immigration.

The effective spawn gate combines:

- manual or trend-controlled immigration intensity;
- optional manual daily new-resident cap;
- optional manual resident population ceiling.

Intensity is implemented as a deterministic duty cycle over household-spawn opportunities. When the system first takes control it stores the existing `HouseholdSpawnSystem.Enabled` state and restores it when control is released.

Direct growth-lock injection does not depend on positive residential demand, but manual population ceilings are still respected by direct correction when manual immigration control is enabled.

## Localization

On load, the mod registers one localization source for every locale supported by the running game.

Portuguese locales use `LocalePTBR`. Other locales currently receive the complete English sources as fallback. Because sources are registered against the game's locale IDs, switching the active game locale selects the matching mod dictionary without maintaining a separate language setting inside Cim Rejuvenator.

## Options and diagnostics

The Options page is generated from the public properties on `CimRejuvenatorSetting`.

Important diagnostics include:

- loaded DLL version;
- established resident census;
- daily and smoothed trend;
- protected growth floor;
- current direct shortfall;
- pending direct residents;
- scheduled direct residents and households;
- effective immigration and birth controls.

The direct Linux build writes `BUILD_INFO.txt`, verifies the deployed DLL SHA-256 checksum, and warns about duplicate local `CimRejuvenator.dll` files.

## Build modes

The project can use the official modding toolchain when `CSII_TOOLPATH` is available. It can also compile directly against the installed game's managed assemblies by setting `ForceNoUnityBuild=true`.

Direct-assembly build scripts remove previous build products before compilation so stale binaries cannot be silently redeployed after source changes.

## Compatibility boundaries

Birth control overlaps with mods that write the same `CitizenParametersData` fields.

Immigration control overlaps with mods that directly enable or disable `HouseholdSpawnSystem`.

Direct growth lock uses the vanilla household prefab/archetype initialization path. Mods that replace household spawning, household prefabs, outside-connection behavior, or household initialization can alter its results.

Demographic balancing changes life stages on existing entities. It preserves entity and household links, but downstream simulation systems can react to the new life stage by changing employment, education, travel, and service demand.

Forced population outflow adds the base game's `MovingAway` component to resident households. The base game's moving-away pipeline then handles the actual departure.
