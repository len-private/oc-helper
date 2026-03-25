# OC-Helper Architecture Guide

## Overview

OC-Helper is a clean, modular Dalamud automation plugin for Occult Crescent farming. Unlike BOCCHI, this plugin prioritizes **code clarity**, **proper configuration handling**, and **maintainability**.

## Project Structure

```
SamplePlugin/
├── Configuration.cs          # Config classes with clear serialization
├── Plugin.cs                 # Main plugin entry point & service coordinator
├── Services/
│   ├── TreasureHuntingService.cs     # Treasure detection & tracking
│   ├── CombatAutomationService.cs    # Combat automation & buffs
│   └── PerformanceTrackerService.cs  # Metrics & statistics
└── Windows/
    ├── ConfigWindow.cs       # Settings UI
    └── MainWindow.cs         # Main automation display
```

## Key Design Principles

### 1. **Clear Separation of Concerns**
Each service handles ONE responsibility:
- **TreasureHuntingService** - Only detects treasures and provides positions
- **CombatAutomationService** - Only handles combat logic and safety checks
- **PerformanceTrackerService** - Only tracks statistics

### 2. **Configuration First**
All settings are defined in `Configuration.cs` with XML documentation:
```csharp
public bool AutoPathfind { get; set; } = true;  // Easy to understand toggle
public float DetectionRadius { get; set; } = 100f;  // Clear purpose & default
```

This avoids BOCCHI's config mapping issues because:
- Settings are strongly-typed
- Defaults are explicit
- No magic string keys
- Easy to serialize/deserialize

### 3. **Framework-Driven Updates**
Services update every frame via `Framework.Update` hook:
```csharp
Framework.Update += OnFrameworkUpdate;  // Called ~60x per second
```

This keeps logic synchronized and responsive.

### 4. **Comprehensive Documentation**
Every class, method, and important property has XML comments explaining:
- **What** it does
- **Why** it exists
- **Usage** examples

## How to Use the Services

### From the UI (ConfigWindow, MainWindow)

Access services through the Plugin instance:

```csharp
public class MainWindow : Window
{
    private readonly Plugin _plugin;

    public MainWindow(Plugin plugin) : base("OC-Helper")
    {
        _plugin = plugin;
    }

    public void DrawUI()
    {
        // Get treasure service
        var treasureService = _plugin.GetTreasureService();
        
        // Display nearby treasures
        ImGui.Text($"Nearby treasures: {treasureService.NearbyTreasures.Count}");
        
        foreach (var treasure in treasureService.NearbyTreasures)
        {
            ImGui.Text($"  - Distance: {treasure.Distance:F1} yalms");
        }
    }
}
```

### Adding New Features

To add a new automation feature:

1. **Create a config class** in `Configuration.cs`:
```csharp
[Serializable]
public class NewFeatureConfig
{
    /// <summary>Enable this feature</summary>
    public bool Enabled { get; set; } = false;
    
    /// <summary>Some setting</summary>
    public float SomeSetting { get; set; } = 50f;
}
```

2. **Create a Service** in `Services/`:
```csharp
public sealed class NewFeatureService : IDisposable
{
    private readonly Configuration _config;
    private readonly IPluginLog _log;

    public NewFeatureService(Configuration config, IPluginLog log)
    {
        _config = config;
        _log = log;
    }

    public void Update()
    {
        if (!_config.NewFeature.Enabled)
            return;
        
        // Your logic here
    }

    public void Dispose() { }
}
```

3. **Integrate into Plugin.cs**:
```csharp
private NewFeatureService NewFeatureService { get; init; }

public Plugin()
{
    // ... other init code ...
    NewFeatureService = new NewFeatureService(Configuration, Log);
}

private void OnFrameworkUpdate(IFramework framework)
{
    NewFeatureService.Update();
}

public void Dispose()
{
    NewFeatureService?.Dispose();
}
```

## Configuration Serialization (Why It Works)

The configuration system uses Dalamud's built-in serialization:

```csharp
// Loading: Dalamud automatically deserializes to Configuration
Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

// Saving: Call from anywhere
Configuration.Save();  // Calls PluginInterface.SavePluginConfig(this)
```

**Why this avoids BOCCHI's problems:**
- ✅ Strongly-typed (compiler checks property names)
- ✅ No manual JSON parsing
- ✅ Nested classes serialize automatically
- ✅ Defaults are obvious in code
- ✅ Can be validated in setter

## Common Patterns

### Pattern: Safety Check Before Action
```csharp
public void Update()
{
    if (!_config.Feature.Enabled)
        return;  // Feature disabled
    
    var player = _clientState.LocalPlayer;
    if (player == null)
        return;  // Not in-game
    
    if (player.CurrentHp < MinimumHp)
        return;  // Safety threshold

    // Safe to proceed with automation
}
```

### Pattern: Finding Objects
```csharp
foreach (var obj in _objectTable)
{
    if (obj is not DesiredType target)
        continue;
    
    if (target.IsDead || target.IsFriendly)
        continue;

    var distance = Vector3.Distance(player.Position, obj.Position);
    if (distance > MaxRange)
        continue;

    // Found usable object
}
```

### Pattern: Update Rates
```csharp
private void UpdateRates()
{
    var elapsed = DateTime.Now - _startTime;
    if (elapsed.TotalSeconds < 1)
        return;
    
    var hoursElapsed = elapsed.TotalHours;
    if (hoursElapsed > 0)
    {
        ItemsPerHour = (uint)(_totalItems / hoursElapsed);
    }
}
```

## Debugging

### Enable Logging
Services use `IPluginLog` for debugging:
```csharp
_log.Information("Feature enabled");
_log.Warning("Low HP threshold breached");
_log.Error("Failed to find target");
```

View logs in-game with `/xllog`.

### Add Debug Output
```csharp
// In service Update()
_log.Debug($"Treasures found: {NearbyTreasures.Count}");
_log.Debug($"Closest: {GetClosestTreasure()?.Distance:F1} yalms");
```

## Common Issues

### Issue: Service updates not running
**Check:** Is `Framework.Update` hooked in Plugin.cs?
```csharp
Framework.Update += OnFrameworkUpdate;  // Must be in constructor
```

### Issue: Config not persisting
**Check:** Called `Configuration.Save()` after changes?
```csharp
_config.TreasureHunting.Enabled = true;
_config.Save();  // Don't forget this!
```

### Issue: Can't access player data
**Check:** Is `ClientState.LocalPlayer` null?
```csharp
var player = _clientState.LocalPlayer;
if (player == null)
    return;  // Not in-game yet
```

## Building on This Foundation

This architecture makes it easy to:
- ✅ Add new automation features (just create a Service)
- ✅ Test in isolation (services have no dependencies on UI)
- ✅ Track what's happening (clear code + logging)
- ✅ Modify behavior (configuration, not magic numbers)
- ✅ Collaborate (clear responsibility lines)

## Next Steps

1. Update `Windows/ConfigWindow.cs` to display treasure/combat settings
2. Update `Windows/MainWindow.cs` to show automation status & metrics
3. Implement TODO items in Services (actual pathfinding, buff casting, etc.)
4. Test each service independently
5. Add more features using the established patterns
