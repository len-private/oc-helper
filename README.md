# OC-Helper: Clean Dalamud Automation for Occult Crescent

> A modular, well-documented Dalamud plugin for automating Occult Crescent farming—built with clarity and maintainability in mind.

## Why OC-Helper?

Unlike BOCCHI and similar plugins, OC-Helper prioritizes:

- ✅ **Clear Code**: Every class and method is documented explaining what it does and why
- ✅ **Working Configuration**: No broken config mapping—strong types, clear defaults
- ✅ **Modular Design**: Each automation feature is a separate service with one responsibility
- ✅ **Easy to Extend**: Add new features by following established patterns
- ✅ **Maintainable**: Code you can understand and modify without confusion

## Features

### Current
- **Treasure Detection & Tracking** - Automatically find nearby treasures
- **Combat Automation** - Aggro management with safety checks
- **Performance Metrics** - Track gil/exp earned per session
- **Configurable Behaviors** - Disable safety features, adjust ranges, etc.

### Planned
- Auto-pathfinding to treasures
- Automatic buff casting (Bard, Knight, Monk)
- Inventory management & loot filtering
- Advanced combat rotations

## Installation

1. Clone this repository
2. Build with `dotnet build`
3. Copy the compiled plugin to your Dalamud plugins directory
4. Enable the plugin in `/xlplugins`

## Quick Start

```
/oc              - Toggle main automation window
/xlconfig oc     - Open configuration window
/xllog           - View plugin logs
```

## Architecture

The plugin is organized into **three layers**:

### Configuration Layer (`Configuration.cs`)
Defines all settings with XML documentation and sensible defaults:
```csharp
public TreasureHuntingConfig TreasureHunting { get; set; } = new();
```

### Service Layer (`Services/`)
Business logic separate from UI—each service handles one feature:
- `TreasureHuntingService` - Detect and track treasures
- `CombatAutomationService` - Manage combat automation
- `PerformanceTrackerService` - Track statistics

### UI Layer (`Windows/`)
ImGui windows displaying status and configuration

**Why this matters**: You can test services independently, replace services without affecting UI, and understand exactly what each piece does.

## Understanding the Code

Start here:
1. Read `ARCHITECTURE.md` for the big picture
2. Look at `Configuration.cs` to see what features are available
3. Examine `Services/TreasureHuntingService.cs` as an example service
4. Open `Plugin.cs` to see how services are integrated

Each file has comments explaining the "why" behind implementation choices.

## Extending OC-Helper

To add a new feature:

1. Define config in `Configuration.cs`:
   ```csharp
   [Serializable]
   public class MyNewFeatureConfig
   {
       public bool Enabled { get; set; } = false;
       public float Setting1 { get; set; } = 50f;
   }
   ```

2. Create a service in `Services/MyNewFeatureService.cs`:
   ```csharp
   public sealed class MyNewFeatureService : IDisposable
   {
       public void Update() { /* Your logic */ }
       public void Dispose() { }
   }
   ```

3. Integrate in `Plugin.cs`:
   - Create the service in the constructor
   - Call its `Update()` method in `OnFrameworkUpdate()`
   - Dispose it in `Dispose()`

See `ARCHITECTURE.md` for detailed patterns and examples.

## Configuration Storage

Settings are saved to:
```
%APPDATA%\XIVLauncher\pluginConfigs\OC-Helper\SamplePlugin.json
```

The JSON structure mirrors your `Configuration.cs` classes:
```json
{
  "Version": 1,
  "TreasureHunting": {
    "Enabled": true,
    "DetectionRadius": 100.0,
    "AutoPathfind": true
  },
  "CombatAutomation": {
    "Enabled": false,
    "SafetyHpThreshold": 30
  }
}
```

No custom mapping needed—Dalamud handles serialization automatically.

## Debugging

View detailed logs in-game:
```
/xllog
```

Each service logs its actions:
- `Information` - Important events (feature enabled, session started)
- `Warning` - Issues that don't break automation (low HP, no treasures)
- `Error` - Critical failures (check these!)

Add your own logs:
```csharp
_log.Debug($"Found {treasures.Count} treasures");
```

## Building

### Prerequisites
- .NET 8 SDK
- Dalamud dependency (included in project)

### Build
```bash
cd SamplePlugin
dotnet build
```

### Output
Compiled plugin: `bin/Release/SamplePlugin/SamplePlugin.json` (+ .dll)

## License

AGPL-3.0 - See LICENSE.md

## Contributing

Want to improve OC-Helper? The codebase is designed to be understandable and modular:

1. Pick an issue or feature
2. Follow the patterns in existing services
3. Document your changes with XML comments
4. Test your service independently
5. Submit a PR

## Comparison: Why This Design Works Better

| Aspect | BOCCHI | OC-Helper |
|--------|--------|-----------|
| Config | Magic strings, broken mapping | Strong types, auto-serialization |
| Code clarity | Dense, hard to follow | Documented, clear flows |
| Architecture | Monolithic | Service-based, testable |
| Adding features | Unclear patterns | Established, easy-to-follow patterns |
| Debugging | Hard to trace | Comprehensive logging |

## Known Limitations

- Pathfinding is not yet implemented (uses basic positioning)
- Buff casting requires manual job setup
- Only works in Occult Crescent (limited by navigation)

## Support

For issues, questions, or improvements:
1. Check `ARCHITECTURE.md` for patterns
2. Read the XML documentation in source files
3. Enable `/xllog` and look for error messages
4. File an issue on GitHub

---

**Happy farming!** 🎮


* XIVLauncher, FINAL FANTASY XIV, and Dalamud have all been installed and the game has been run with Dalamud at least once.
* XIVLauncher is installed to its default directories and configurations.
  * If a custom path is required for Dalamud's dev directory, it must be set with the `DALAMUD_HOME` environment variable.
* A .NET Core 8 SDK has been installed and configured, or is otherwise available. (In most cases, the IDE will take care of this.)

### Building

1. Open up `SamplePlugin.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
3. The resulting plugin can be found at `SamplePlugin/bin/x64/Debug/SamplePlugin.dll` (or `Release` if appropriate.)

### Activating in-game

1. Launch the game and use `/xlsettings` in chat or `xlsettings` in the Dalamud Console to open up the Dalamud settings.
    * In here, go to `Experimental`, and add the full path to the `SamplePlugin.dll` to the list of Dev Plugin Locations.
2. Next, use `/xlplugins` (chat) or `xlplugins` (console) to open up the Plugin Installer.
    * In here, go to `Dev Tools > Installed Dev Plugins`, and the `SamplePlugin` should be visible. Enable it.
3. You should now be able to use `/pmycommand` (chat) or `pmycommand` (console)!

Note that you only need to add it to the Dev Plugin Locations once (Step 1); it is preserved afterwards. You can disable, enable, or load your plugin on startup through the Plugin Installer.

### Reconfiguring for your own uses

Replace all references to `SamplePlugin` in all the files and filenames with your desired name, then start building the plugin of your dreams. You'll figure it out 😁

Dalamud will load the JSON file (by default, `SamplePlugin/SamplePlugin.json`) next to your DLL and use it for metadata, including the description for your plugin in the Plugin Installer. Make sure to update this with information relevant to _your_ plugin!
