# Windows Build Guide

Cim Rejuvenator can be compiled locally without opening the Unity editor. The direct-assembly build references the DLLs installed with Cities: Skylines II.

> Back up important saves before testing a new development build.

## Requirements

Install Git and the .NET SDK from an elevated PowerShell window:

```powershell
winget install Git.Git
winget install Microsoft.DotNet.SDK.10
```

Open a new PowerShell window and verify:

```powershell
git --version
dotnet --version
```

## Clone or update the repository

First clone:

```powershell
cd C:\Users\$env:USERNAME
git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator
```

Existing checkout:

```powershell
cd C:\Users\$env:USERNAME\CimRejuvenator
git pull
```

Confirm the current project version:

```powershell
Select-String '<Version>' .\CimRejuvenator.csproj
```

The current release should report `0.4.0`.

## PowerShell execution policy

If local scripts are blocked, allow them for the current PowerShell process only:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
```

## Check the environment

```powershell
.\check-environment.ps1
```

The checker reports whether the direct-assembly build, the official toolchain build, or both are available.

## Build without Unity

```powershell
.\build-no-unity.ps1
```

The direct-build script removes previous `bin`, `obj`, and `dist` output before compiling, then writes:

```text
dist\CimRejuvenator\CimRejuvenator.dll
dist\CimRejuvenator\BUILD_INFO.txt
```

`BUILD_INFO.txt` records the project version, source commit, and SHA-256 checksum.

For a custom Steam Library, set `CSII_GAMEPATH`:

```powershell
$env:CSII_GAMEPATH="D:\SteamLibrary\steamapps\common\Cities Skylines II"
.\build-no-unity.ps1
```

The path is valid when this file exists:

```text
%CSII_GAMEPATH%\Cities2_Data\Managed\Game.dll
```

## Official toolchain build

If `CSII_TOOLPATH`, `Mod.props`, and `Mod.targets` are installed and available:

```powershell
.\build.ps1
```

The official toolchain is useful for the standard Cities: Skylines II publishing pipeline. Local direct-assembly builds are sufficient for development testing.

## Copying a Windows build to Linux / Proton

Copy both files from:

```text
dist\CimRejuvenator\
```

to the local mod directory in the Proton prefix:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Mods/CimRejuvenator/
```

The final directory should contain:

```text
CimRejuvenator.dll
BUILD_INFO.txt
```

Do not place local development DLLs under `.cache/Mods/pdx_mods`; that directory is managed by Paradox Mods.

## First v0.4.0 test

The **General** group should show:

```text
Loaded build version: 0.4.0
```

If that row is missing, an older DLL is being loaded or the game has not been fully restarted since the DLL was replaced.

Confirm the resident census and rejuvenation system first, then enable the new controllers individually.

Suggested initial settings:

```text
Enable Cim Rejuvenator:             Yes
Enable rejuvenation:                Yes
Rejuvenation chance:                80%
Age after rejuvenation:             40
Maximum rejuvenations/day:          20,000
Maximum rejuvenations/sweep:        5,000
Population sweeps/day:              64

Demographic balancer:               No
Population trend controller:        No
Immigration control:                No
Birth control:                      No
```

After the census is stable, a reasonable demographic target is:

```text
Child:      15
Teen:       10
Adult:      60
Elderly:    15
```

The four target values are normalized automatically.

A conservative population-trend test is:

```text
Population trend controller:        Yes
Target net change/day:              0
Response strength:                  50%
Deadband:                           500
Use immigration:                    Yes
Use births:                         Yes
Maximum automatic birth rate:       250%
Forced outflow:                     No
```

The controller needs at least one complete simulation-day transition after activation to establish a useful trend sample.

## Build failures

Capture the complete output:

```powershell
.\build-no-unity.ps1 *> build-error.txt
```

A `CSxxxx` compiler error usually indicates a game API field, namespace, or type that differs from the version targeted by the source.

## Updating

```powershell
cd C:\Users\$env:USERNAME\CimRejuvenator
git pull
Set-ExecutionPolicy -Scope Process Bypass
.\build-no-unity.ps1
```

Close Cities: Skylines II before replacing a loaded code-mod DLL, then restart it and confirm the loaded build version in Options.
