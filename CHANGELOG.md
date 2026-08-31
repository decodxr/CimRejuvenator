# Changelog

## Unreleased

### Changed

- Replaced the MIT license for current and future distributions with the Cim Rejuvenator Noncommercial Attribution License 1.0.
- Redistribution and derivative works now require visible credit to Cim Rejuvenator by decodxr and preservation of the project license and notice.
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
