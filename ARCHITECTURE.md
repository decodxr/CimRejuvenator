# Architecture

Cim Rejuvenator is split into four simulation systems so population conversion, flow tracking, birth-rate control, and immigration gating can be enabled independently.

## PopulationManagementSystem

Runs a configurable number of full resident sweeps per simulation day.

Responsibilities:

- Build the resident census by `CitizenAge`.
- Exclude tourists, commuters, deleted entities, temporary entities, and dead citizens from demographic counts.
- Rejuvenate eligible Elderly residents.
- Apply optional minimum-Elderly-share protection.
- Move the population toward normalized Child / Teen / Adult / Elderly target weights.
- Enforce per-sweep conversion limits.
- Preserve employed Adults when worker protection is enabled.

Life-stage conversions use `Citizen.SetAge()` and update `Citizen.m_BirthDay` to keep the age bits and calculated age consistent.

## PopulationFlowSystem

Maintains a session-local set of observed citizen entity IDs.

The first update establishes a baseline. Later newly created living resident entities are classified as:

- newborns when they are Child residents with calculated age zero;
- incoming residents otherwise.

When incoming age shaping is enabled, newly detected immigrants are reassigned using the configured age weights.

The daily birth and immigration limits are intentionally described as soft caps. Detection happens after entities are created, so a household or birth operation already in progress can finish beyond the selected threshold.

## BirthRateControlSystem

Captures the original `CitizenParametersData` values before applying birth control.

The requested birth multiplier scales:

- `m_BaseBirthRate`
- `m_AdultFemaleBirthRateBonus`

`m_StudentBirthRateAdjust` is left unchanged because it is a separate multiplier in the game's birth calculation.

The effective multiplier becomes zero when either enabled stop condition is reached:

- daily birth cap;
- normalized Child demographic target.

Original parameter values are restored when birth control is disabled or the system is destroyed.

## ImmigrationControlSystem

Controls `HouseholdSpawnSystem.Enabled`.

The spawn gate combines:

- immigration intensity;
- optional daily new-resident cap;
- optional resident population ceiling.

Intensity is implemented as a deterministic duty cycle over household-spawn opportunities. This avoids replacing the game's household creation pipeline and leaves housing selection, household initialization, property seeking, and outside-connection behaviour to the base game.

## Build modes

The project can use the official modding toolchain when `CSII_TOOLPATH` is available. It can also compile directly against the installed game's managed assemblies by setting `ForceNoUnityBuild=true`.

Direct-assembly builds are intended for local development and compatibility testing. The official toolchain remains the preferred path for standard publishing workflows.

## Compatibility boundaries

Birth control overlaps with mods that write the same `CitizenParametersData` fields.

Immigration control overlaps with mods that directly enable or disable `HouseholdSpawnSystem`.

Demographic balancing changes life stages on existing entities. It preserves the entity and household links, but downstream simulation systems can react to the new life stage by changing employment, education, travel, and service demand.
