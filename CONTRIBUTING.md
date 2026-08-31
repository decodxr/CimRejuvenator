# Contributing

Contributions, compatibility reports, and focused bug fixes are welcome.

## Before opening an issue

Include the following information when reporting a runtime or build problem:

- Cities: Skylines II version.
- Operating system and Proton version when applicable.
- Cim Rejuvenator version or commit SHA.
- Whether the build used the official toolchain or direct game assemblies.
- Other population, fertility, immigration, lifecycle, or simulation mods that were enabled.
- Relevant lines from `CimRejuvenator.log` and `Modding.log`.

For compiler failures, attach or paste the output produced by:

### Linux

```bash
./build-no-unity-linux.sh > build-error.txt 2>&1
```

### Windows

```powershell
.\build-no-unity.ps1 *> build-error.txt
```

## Development guidelines

- Keep repository text, code comments, logs, and built-in localization in English.
- Keep population limits bounded and expose aggressive behaviour behind explicit settings.
- Preserve citizen entities and household links whenever possible.
- Restore modified game parameters when a controller is disabled or destroyed.
- Document interactions with vanilla systems and known mod conflicts.
- Avoid adding game assemblies or other proprietary game files to the repository.

## Testing population changes

Use a copied save for changes that alter citizen life stages, birth parameters, or household spawning.

Test one controller at a time before combining them:

1. Population census and rejuvenation.
2. Demographic balancing.
3. Immigration control.
4. Birth control.

Verify the Options statistics and logs after each step.
