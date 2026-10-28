# CS2 Save Fixer Mod

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](#)
[![Cities: Skylines II](https://img.shields.io/badge/Cities:%20Skylines%20II-Mod-blue)](#)
[![C# 10.0](https://img.shields.io/badge/C%23-10.0-purple)](#)

**CS2 Save Fixer Mod** (`CS2SaveFixer.Mod`) is an emergency save recovery and runtime protection code mod for **Cities: Skylines II**. It intercepts missing prefab GUID errors during save loading, safely bypasses corrupted deserialization chunks, and runs automated ECS sanitization passes to recover corrupted save files.

> **Note:** This repository contains the standalone runtime Cities: Skylines II code mod. It originated from the broader save repair project [CS2-GameSaveFixer](https://github.com/BK1233/CS2-GameSaveFixer) and is maintained in this dedicated repository for modular mod development and distribution.

---

## Key Features

- **Safe Deserialization Interceptor (`DeserializationInterceptor`)**
  - Intercepts missing prefab GUIDs during world deserialization and safely remaps missing assets (buildings, roads, vehicles) to base-game fallback defaults instead of crashing to desktop or failing to load.
  - Catches deserialization exceptions on corrupted entity components and safely bypasses damaged chunk data to preserve overall world loading.

- **Automated ECS Sanitization (`SaveFixerCleanupSystem`)**
  - Scans simulation entities for corrupted floating-point vector coordinates (`NaN` / `Infinity`) and resets them to valid origin coordinates.
  - Purges orphaned entities and invalid prefab references during world cleanup passes.

- **In-Game Settings UI Integration (`Setting.cs`)**
  - Integrated directly into the Cities: Skylines II options interface for easy toggles and configuration.

---

## Installation & Usage

### Installing via Paradox Mods
When published, **CS2 Save Fixer** can be subscribed to directly through the Paradox Mods interface in Cities: Skylines II.

### Manual Installation
1. Download the latest `CS2SaveFixer.Mod.dll` from the Releases section.
2. Place the compiled mod folder into your local Cities: Skylines II mod directory:
   ```
   %LOCALAPPDATA%Low\Colossal Order\Cities Skylines II\Mods\CS2SaveFixer.Mod
   ```
3. Launch Cities: Skylines II. The mod will automatically register during the game boot phase.

---

## Building from Source

### Prerequisites
- **.NET 8.0 SDK** (or higher)
- **Cities: Skylines II** with the official Modding Toolchain installed (`CSII_TOOLPATH` set)
- **Visual Studio 2022** (or VS Code) with C# 10.0 support

### Build Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/BK1233/CS2SaveFixer.Mod.git
   cd CS2SaveFixer.Mod
   ```
2. Build the solution using `dotnet`:
   ```bash
   dotnet build CS2SaveFixer.Mod.csproj -c Debug
   ```
   *Note: Upon successful compilation, the game's post-processor toolchain (`ModPostProcessor.exe`) will process the output DLL with Unity Burst/Entities ILPP and deploy it directly to your local game mods directory.*

3. Run Unit Tests:
   ```bash
   dotnet test CS2SaveFixer.Mod.Tests/CS2SaveFixer.Mod.Tests.csproj
   ```

---

## Project Structure

```
CS2SaveFixer.Mod/
├── CS2SaveFixer.Mod.csproj       # Main C# project configuration
├── CS2SaveFixer.Mod.sln          # Visual Studio solution
├── Mod.cs                         # Primary IMod entry point & setting registration
├── SaveFixerMod.cs                # Secondary runtime system initializer
├── DeserializationInterceptor.cs  # Harmony/deserialization exception handling & fallback resolver
├── SaveFixerCleanupSystem.cs      # ECS world sanitization & entity cleanup pass
├── Setting.cs                     # Options UI & localization sources
├── mod.json                       # Mod manifest metadata
├── PublishConfiguration.xml       # Paradox Mods publishing configuration
└── CS2SaveFixer.Mod.Tests/        # Standalone xUnit test suite with stubbed game assemblies
```

---

## Related Projects

- **[CS2-GameSaveFixer](https://github.com/BK1233/CS2-GameSaveFixer)** — Original project repository for save diagnostic tools and save file fix utilities.

---

## License & Credits

Developed by the **CS2SaveFixer Team**. Built for the **Cities: Skylines II** modding community.
