# Changelog

## 0.6.1

### Fixed

- Direct and adaptive trend control now read `Game.City.Population.m_Population`, the same population component used by the vanilla city UI, instead of relying only on the mod's filtered resident census.
- Direct growth checks increased from 64 to 256 per simulation day for faster response to sudden population loss.
- Direct mode now estimates short-window hourly population movement and adds a recovery reserve when the visible city population is still falling, helping the controller push past break-even instead of only replacing the latest loss.
- Pending direct residents are credited more conservatively so delayed household initialization cannot make the controller believe a population loss has already been recovered.
- Trend logs now report vanilla population, managed resident census, estimated hourly movement, protected floor, shortfall, scheduled residents, and pending residents.

## 0.6.0

### Added

- Continuous direct growth lock that reacts throughout the simulation day instead of waiting for one daily correction.
- A non-decreasing high-water population floor for zero and positive direct targets.
- Positive direct targets now create a protected upward trajectory across the day.
- Separate direct-injection limits per day and per correction check.
- Emergency growth preset for severe population collapse.
- Diagnostics for protected population floor, current shortfall, and pending direct residents.
- Portuguese localization selected automatically for Portuguese game locales.
- English fallback registration for other supported game locales.

### Changed

- Population trend target range increased from +/-100,000 to +/-500,000 residents per day.
- Maximum direct-injection capacity increased to 1,000,000 estimated residents per day.
- Birth-rate controller and automatic trend birth-rate ceiling increased to 1,000%.
- Immigration daily cap increased to 1,000,000 and resident population ceiling increased to 5,000,000.
- Direct mode trusts only part of not-yet-established injected population so it retries more aggressively during sustained death waves.
- Pending direct population is partially retried across day boundaries instead of being trusted forever.
- Positive direct mode no longer depends on the previous full-day trend before it can react to a current population drop.

### Behavior notes

- A direct target of `0/day` acts as a no-decline lock.
- A direct positive target both replaces observed losses and advances the protected population floor.
- Direct household counts remain estimates until the normal household initialization and move-in pipeline establishes the residents.

## 0.5.0

### Added

- Direct population-trend compensation mode.
- Direct mode measures the previous complete simulation day's established-resident change and schedules normal vanilla resident households when the city falls below the configured target.
- Configurable direct correction strength from 10% to 100% of the measured shortfall.
- Configurable direct-injection safety cap up to 250,000 estimated residents per simulation day.
- Direct-mode statistics for requested correction, estimated residents scheduled, and households scheduled.
- Incoming age shaping can now apply to residents created by direct trend compensation even when manual immigration control is disabled.

### Changed

- Adaptive trend control remains available as the less invasive mode.
- In direct mode, immigration and birth-rate controls act as optional assist channels while household injection provides the strict shortfall correction.
- When actual growth exceeds the selected target, direct mode throttles assist channels. Forced household outflow remains separately opt-in.
- Direct household injection uses normal household prefabs, outside connections, `PrefabRef`, `CurrentBuilding`, and the game's household initialization pipeline instead of creating citizen entities manually.

### Fixed

- Added the missing `Game.Common` namespace imports required for `TimeData` in birth-rate and immigration controllers.

### Safety notes

- Direct compensation can create substantial housing, traffic, employment, education, and service demand. Use a save backup when testing large corrections.
- Scheduled resident counts are estimates based on household prefab composition. The final established population is still completed by the normal household initialization and moving-in simulation.

## 0.4.0

### Added

- Adaptive population trend controller with a configurable net resident-change target from -100,000 to +100,000 residents per simulation day.
- Trend response strength and deadband controls.
- Optional immigration and birth-rate channels for automatic trend correction.
- Configurable maximum automatic birth-rate multiplier.
- Optional forced household outflow for negative population targets, disabled by default.
- Trend statistics for actual daily change, smoothed trend, effective immigration, effective birth rate, and forced outflow.
- Loaded build version displayed directly in the Options page.
- Build metadata and SHA-256 checksum output for direct-assembly builds.
- Duplicate local DLL detection during Linux deployment.

### Fixed and hardened

- Direct Linux and Windows builds now delete `bin`, `obj`, and `dist` before compiling so an old DLL cannot be redeployed after source changes.
- Linux deployment now replaces the local mod directory and verifies the deployed DLL checksum against the freshly built DLL.
- Settings localization and persisted values are loaded before the Options page is registered.
- Resident census, demographic management, and population-flow tracking now require moved-in resident households and exclude households already moving away.
- Population-flow tracking was reduced from 512 to 128 full checks per day and periodically prunes stale citizen keys.
- Immigration control restores the previous `HouseholdSpawnSystem.Enabled` state when it releases control instead of always forcing the vanilla spawner on.
- Demographic balancing skips residents in active trips or enrolled as students and clears partner-seeking state when converting to Child or Teen.
- Incoming age shaping clears partner-seeking state when assigning Child or Teen.

### Licensing

- Current and future distributions use the Cim Rejuvenator Noncommercial Attribution License 1.0.
- Redistribution and derivative works require visible credit to Cim Rejuvenator by decodxr and preservation of the project license and notice.
- Commercial distribution, paid access, sale, and monetization of the software or derivative works are prohibited without prior written permission.
- Previously distributed MIT copies remain governed by the license that accompanied those copies.

## 0.3.0

### Added

- Demographic balancer with configurable Child, Teen, Adult, and Elderly target weights.
- Manual demographic balancing action.
- Configurable maximum demographic conversions per population sweep.
- Worker-protection option during demographic balancing.
- Immigration intensity control through the resident household spawn system.
- Optional daily new-resident soft cap.
- Optional resident population ceiling.
- Incoming resident age-mix shaping.
- Birth-rate multiplier from 0% to 500%.
- Optional daily birth soft cap.
- Optional birth pause when the Child demographic target is reached.
- Resident census by life stage.
- Birth and incoming-resident session statistics.
- Immigration controller status and applied birth-rate statistics.
- `CSII_USER_DATA` override for Linux deployment.
- Repository ignore rules.

### Changed

- Rejuvenation and census logic moved into `PopulationManagementSystem`.
- Population sweeps remain configurable from 8 to 256 per simulation day.
- Repository documentation, scripts, comments, and built-in localization are standardized on English.
- Project version updated to 0.3.0.

### Compatibility notes

- Birth/fertility mods that also write `CitizenParametersData` can conflict with Cim Rejuvenator birth control.
- Immigration mods that also enable or disable `HouseholdSpawnSystem` can conflict with Cim Rejuvenator immigration control.
- Daily birth and immigration caps are soft caps because the game can finish work already in progress before a controller closes the gate.

## 0.2.0

- Added immediate rejuvenation requests.
- Increased rejuvenation limits.
- Added a minimum Elderly-share safeguard.
- Added population sweep statistics.
- Added direct Linux build and Proton deployment support.

## 0.1.x

- Initial configurable Elderly-to-Adult rejuvenation implementation.
- Added direct-assembly build support for local development without Unity editor activation.
