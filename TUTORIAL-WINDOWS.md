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

## Clone the repository

```powershell
cd C:\Users\$env:USERNAME
git clone https://github.com/decodxr/CimRejuvenator.git
cd CimRejuvenator
```

For an existing checkout:

```powershell
cd C:\Users\$env:USERNAME\CimRejuvenator
git pull
```

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

The script checks common Steam Library locations. For a custom library, set `CSII_GAMEPATH`:

```powershell
$env:CSII_GAMEPATH="D:\SteamLibrary\steamapps\common\Cities Skylines II"
.\build-no-unity.ps1
```

The path is valid when this file exists:

```text
%CSII_GAMEPATH%\Cities2_Data\Managed\Game.dll
```

The packaged DLL is written to:

```text
dist\CimRejuvenator\CimRejuvenator.dll
```

## Official toolchain build

If `CSII_TOOLPATH`, `Mod.props`, and `Mod.targets` are installed and available:

```powershell
.\build.ps1
```

The official toolchain is useful for the standard Cities: Skylines II publishing pipeline. Local direct-assembly builds are sufficient for development testing.

## Copying a Windows build to Linux / Proton

Copy:

```text
dist\CimRejuvenator\CimRejuvenator.dll
```

to the local mod directory in the Proton prefix:

```text
~/.local/share/Steam/steamapps/compatdata/949230/pfx/drive_c/users/steamuser/AppData/LocalLow/Colossal Order/Cities Skylines II/Mods/CimRejuvenator/
```

The final path should be:

```text
.../Mods/CimRejuvenator/CimRejuvenator.dll
```

Do not place local development DLLs under `.cache/Mods/pdx_mods`; that directory is managed by Paradox Mods.

## First v0.3.0 test

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

Close Cities: Skylines II before replacing a loaded code-mod DLL.
