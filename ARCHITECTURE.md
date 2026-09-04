# Architecture

Cim Rejuvenator separates demographic management, population flow, trend control, birth control, immigration gating, and resident death protection into independent ECS systems.

## Death protection

Version 0.7.0 adds a hard death lock for established residents while the master mod switch is enabled.

### DeathProtectionSystem

`DeathProtectionSystem` is scheduled immediately before vanilla `DeathCheckSystem` in `GameSimulation`. It captures the previous `DeathCheckSystem.Enabled` state and forces the vanilla system off while Cim Rejuvenator is active. This prevents the standard old-age and sickness/injury death path from committing deaths.

When control is released, the previous enabled state is restored rather than assuming vanilla should always be enabled.

### DeathSicknessGuardSystem

Runs after `SicknessCheckSystem` in `GameSimulation` and clears any fatal resident health state that is already materialized at that point.

### DeathHealthGuardSystem

Runs after `HealthProblemSystem` in `GameSimulation`. It catches fatal states created by late health, danger, trapped, or related processing.

### DeathEventGuardSystem

Runs after `Game.Events.AddHealthProblemSystem` in `Modification4` to catch disaster and event death states.

### DeathRemovalGuardSystem

Runs immediately before `HouseholdAndCitizenRemoveSystem` in `Modification2`. It is the final safety net and also allows dead residents already present in a loaded save to be revived before the vanilla removal system consumes them.

All guards protect established residents in moved-in households. Tourists and commuters are excluded. A protected resident keeps the same citizen entity and household membership; `Dead`, `RequireTransport`, `InDanger`, `Trapped`, `Sick`, and `Injured` are cleared, the event reference and timer are reset, and health is restored to 100.

Moving-away households are not prevented from completing normal departure, so population can still decrease from genuine out-migration.

Event systems can emit casualty statistics before a guard clears a fatal state. Those counters are not treated as proof that the citizen was removed; resident population and entity survival are the authoritative checks for the death lock.

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

Adaptive mode records population movement across simulation-day boundaries, maintains an exponential moving average, and adjusts enabled control channels toward the selected target:

- effective immigration intensity;
- effective birth-rate multiplier;
- optional forced household outflow for negative targets.

### Continuous direct growth lock

For zero and positive targets, direct mode maintains a high-water population floor and an optional upward target trajectory. It reads `Game.City.Population.m_Population`, matching the vanilla city population indicator.

At each direct check, the controller compares current population plus partial credit for pending direct residents against the protected floor. If the city is short, it schedules resident households subject to correction strength, a per-check limit, and a daily limit.

Only part of pending direct population is trusted when calculating the next shortfall. Pending population is also partially retried at day boundaries so delayed move-in does not make the controller stop correcting too early.

With death protection enabled, direct growth lock is primarily responsible for deliberate growth and compensation for real move-outs rather than replacing normal deaths.

### Direct household injection path

Direct mode does not construct standalone citizen entities. It mirrors the vanilla resident household entry path:

1. query normal household prefabs with `ArchetypeData` and `HouseholdData`, excluding `DynamicHousehold`;
2. select household prefabs using their normal `m_Weight` and estimated household size;
3. create the household from the prefab archetype through `EndFrameBarrier`;
4. set `PrefabRef`;
5. add `CurrentBuilding` pointing to a valid non-utility outside connection;
6. leave citizen creation, household initialization, property seeking, and move-in completion to the normal game pipeline.

Direct resident counters are scheduled estimates until those households become established residents.

For negative direct targets, inflow can be suppressed and forced household outflow remains separately opt-in.

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

On load, the mod registers localization for every locale supported by the running game.

Portuguese locales use `LocalePTBR`. Other currently unsupported locales receive the complete English sources as fallback. Repository code, comments, documentation, and build scripts remain in English.

## Options and diagnostics

The Options page is generated from the public properties on `CimRejuvenatorSetting`. `BuildVersion` exposes the version of the loaded DLL so stale deployments can be identified quickly.

The death lock currently follows the master `Enable Cim Rejuvenator` switch instead of using a second toggle, so there is no state where the population controller is enabled but normal resident death is accidentally left active.

## Build modes

The project can use the official modding toolchain when `CSII_TOOLPATH` is available. It can also compile directly against the installed game's managed assemblies by setting `ForceNoUnityBuild=true`.

Direct-assembly build scripts remove previous build products before compilation, write build metadata, verify the deployed DLL hash, and warn about duplicate local copies.

## Compatibility boundaries

Birth control overlaps with mods that write the same `CitizenParametersData` fields.

Immigration control overlaps with mods that directly enable or disable `HouseholdSpawnSystem`.

Death protection overlaps with mods that replace or take runtime ownership of `DeathCheckSystem`, `SicknessCheckSystem`, `HealthProblemSystem`, `AddHealthProblemSystem`, or resident-removal behavior.

Direct growth lock uses the vanilla household prefab/archetype initialization path. Mods that replace household spawning, household prefabs, outside-connection behavior, or household initialization can alter its results.

Demographic balancing changes life stages on existing entities. It preserves entity and household links, but downstream systems can react to the new life stage by changing employment, education, travel, and service demand.

Forced population outflow adds the base game's `MovingAway` component to resident households. The base game's moving-away pipeline then handles the actual departure.
