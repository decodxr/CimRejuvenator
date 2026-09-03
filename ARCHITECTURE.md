# Architecture

Cim Rejuvenator is split into five simulation systems so population conversion, flow tracking, trend feedback, birth-rate control, and immigration gating remain separated.

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

When incoming age shaping is enabled, newly detected incoming residents are reassigned using the configured life-stage weights.

The scanner runs less frequently than the scheduler used by population management and periodically prunes entity IDs that no longer belong to active resident entities. This keeps the session-local tracking set bounded over long play sessions.

Daily birth and immigration limits are soft caps. Detection happens after entities are created, so a household or birth operation already in progress can finish beyond the selected threshold.

## PopulationTrendSystem

Implements feedback control over the net established-resident population change.

Once enabled, the controller establishes a population baseline and then samples the resident change across complete simulation-day transitions. The measured change is smoothed using an exponential moving average before control adjustments are made.

Inputs:

- target net resident change per simulation day;
- response strength;
- deadband;
- enabled control channels.

Positive and neutral targets can adjust:

- effective immigration intensity;
- effective birth-rate multiplier.

Negative targets first suppress enabled inflow channels. Optional forced outflow can additionally add `MovingAway` to moved-in resident households until a soft daily resident budget is reached. Forced outflow is disabled by default.

The trend controller does not create citizens or households directly. It delegates birth and immigration effects to the existing controllers so the base game's spawning and birth pipelines remain in use.

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

Intensity is implemented as a deterministic duty cycle over household-spawn opportunities. This avoids replacing the game's household creation pipeline and leaves housing selection, household initialization, property seeking, and outside-connection behaviour to the base game.

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

Demographic balancing changes life stages on existing entities. It preserves the entity and household links, but downstream simulation systems can react to the new life stage by changing employment, education, travel, and service demand.

Forced population outflow adds the base game's `MovingAway` component to resident households. The base game's moving-away pipeline then handles the actual departure.
