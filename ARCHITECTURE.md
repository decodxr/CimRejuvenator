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

The first update establishes a baseline. Later newly detected living residents in moved-in households are classified as:

- newborns when they are Child residents with calculated age zero;
- incoming residents otherwise.

When incoming age shaping is enabled, newly detected incoming residents are reassigned using the configured life-stage weights. Age shaping can apply to manual immigration control and to households introduced by direct population-trend compensation.

The scanner periodically prunes entity IDs that no longer belong to active resident entities. Daily birth and immigration limits are soft caps because detection happens after entities are created.

## PopulationTrendSystem

Controls the net established-resident population change across complete simulation-day transitions.

The system supports two modes.

### Adaptive mode

Adaptive mode records the daily resident change, maintains an exponential moving average, and adjusts enabled control channels toward the selected target:

- effective immigration intensity;
- effective birth-rate multiplier;
- optional forced household outflow for negative targets.

It keeps the base game's normal demand-driven population flow and is therefore the less invasive mode, but large death waves can still overwhelm the available inflow.

### Direct compensation mode

Direct mode compares the latest complete day's actual resident change directly with `TargetNetPopulationChangePerDay`.

If the city is below target, the shortfall is converted into a resident correction budget. Correction strength controls how much of that shortfall is addressed, and a separate daily safety cap limits the maximum estimated residents that can be scheduled.

The system does not construct standalone citizen entities. Instead it mirrors the vanilla resident household entry path:

1. query normal household prefabs with `ArchetypeData` and `HouseholdData`, excluding `DynamicHousehold`;
2. select household prefabs using their normal `m_Weight`, while estimating resident count from Child, Adult, Elder, and Student members;
3. create the household using the prefab's archetype;
4. set `PrefabRef`;
5. add `CurrentBuilding` pointing to a valid outside connection;
6. leave citizen creation and household initialization to the game's normal `HouseholdInitializeSystem` pipeline.

The correction counters are therefore estimates/scheduled counts until vanilla household initialization and moving-in complete.

If actual growth is already above the selected target, direct mode can throttle its immigration and birth assist channels. Direct removal remains opt-in through the existing forced-outflow setting.

A configured manual population ceiling is also respected by direct positive correction when manual immigration control is enabled.

## BirthRateControlSystem

Captures the original `CitizenParametersData` values before applying birth control.

The effective birth multiplier scales:

- `m_BaseBirthRate`
- `m_AdultFemaleBirthRateBonus`

`m_StudentBirthRateAdjust` is left unchanged because it is a separate multiplier in the game's birth calculation.

The effective multiplier can come from either the manual birth controller or `PopulationTrendSystem`. Manual birth caps and the normalized Child-target stop condition remain available when manual birth control is enabled.

Original parameter values are restored when no controller needs birth-rate ownership or when the system is destroyed.

## ImmigrationControlSystem

Controls `HouseholdSpawnSystem.Enabled`.

The effective spawn gate combines:

- manual or trend-controlled immigration intensity;
- optional manual daily new-resident cap;
- optional manual resident population ceiling.

Intensity is implemented as a deterministic duty cycle over household-spawn opportunities. This leaves housing selection, household initialization, property seeking, and outside-connection behaviour to the base game for ordinary immigration.

When the system first takes control it stores the existing `HouseholdSpawnSystem.Enabled` state. That state is restored when control is released instead of assuming the correct previous value was always `true`.

## Options and version diagnostics

The Options page is generated from the public properties on `CimRejuvenatorSetting`. A read-only `BuildVersion` row exposes `Mod.Version`, making it possible to distinguish a stale loaded DLL from a source/settings problem.

The direct Linux build also writes `BUILD_INFO.txt`, verifies the deployed DLL SHA-256 checksum, and warns about duplicate local `CimRejuvenator.dll` files.

## Build modes

The project can use the official modding toolchain when `CSII_TOOLPATH` is available. It can also compile directly against the installed game's managed assemblies by setting `ForceNoUnityBuild=true`.

Direct-assembly build scripts remove previous build products before compilation so stale binaries cannot be silently redeployed after source changes.

## Compatibility boundaries

Birth control overlaps with mods that write the same `CitizenParametersData` fields.

Immigration control overlaps with mods that directly enable or disable `HouseholdSpawnSystem`.

Direct trend compensation uses the vanilla household prefab/archetype initialization path. Mods that replace household spawning, household prefabs, outside-connection behavior, or household initialization can alter its results.

Demographic balancing changes life stages on existing entities. It preserves the entity and household links, but downstream simulation systems can react to the new life stage by changing employment, education, travel, and service demand.

Forced population outflow adds the base game's `MovingAway` component to resident households. The base game's moving-away pipeline then handles the actual departure.
