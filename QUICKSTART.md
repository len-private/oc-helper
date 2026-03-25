# Quick Start Guide - OC-Helper Development

Getting up and running with OC-Helper development in 5 minutes.

## Prerequisites

- Visual Studio 2022 OR JetBrains Rider (or VS Code + .NET CLI)
- .NET 8 SDK or later
- XIVLauncher installed
- Admin access to build and deploy

## Step 1: Build the Project

```bash
cd /home/len-s/oc-helper
dotnet build -c Release
```

**Expected output:** 
```
Build succeeded!
Output: SamplePlugin/bin/Release/SamplePlugin/
```

If you get errors, check:
- .NET SDK version: `dotnet --version` should be 8.0+
- All dependencies are NuGet packages (auto-restored)

## Step 2: Deploy to Dalamud

Copy the built plugin to your Dalamud plugin directory:

**Windows:**
```
copy SamplePlugin\bin\Release\SamplePlugin\ 
  "%APPDATA%\XIVLauncher\plugins\SamplePlugin\"
```

**Linux/Mac:**
```bash
cp -r SamplePlugin/bin/Release/SamplePlugin/ \
  ~/.xlcore/plugins/SamplePlugin/
```

## Step 3: Load in Game

1. Start Final Fantasy XIV with XIVLauncher
2. Open plugin installer: `/xlplugins`
3. Find "OC-Helper" in the installed list
4. Click "Enabled" if it's not already
5. Open the main UI: `/oc`

**Check logs:** `/xllog` should show:
```
[OC-Helper] ===OC-Helper Plugin Loaded===
```

## Step 4: Basic Testing

In the plugin windows:
- [ ] Config window opens (`/xlconfig oc`)
- [ ] Main window opens (`/oc`)
- [ ] Settings are editable
- [ ] Settings persist after reload

## Step 5: Run Service Updates

Enable a feature and watch `/xllog`:

1. Open config window: `/xlconfig oc`
2. Set `TreasureHunting.Enabled = true`
3. Watch `/xllog` for messages
4. Look for "Treasures found: X" messages

You should see logs like:
```
[OC-Helper] Update: Treasures found near player
[OC-Helper] Closest treasure: 42.5 yalms away
```

---

## Common Development Tasks

### Add a New Setting

**File:** `Configuration.cs`

```csharp
// 1. Add to existing config class
public class TreasureHuntingConfig
{
    /// <summary>New setting description</summary>
    public int NewSetting { get; set; } = 50;  // ← Default value
}

// 2. Use in service
if (_config.TreasureHunting.NewSetting > threshold)
{
    // Do something
}

// 3. Display in UI (ConfigWindow.cs)
ImGui.SliderInt("New Setting", ref someValue, 0, 100);
```

### Add a New Service

**Option A: Copy from existing service**

```bash
cp SamplePlugin/Services/TreasureHuntingService.cs \
   SamplePlugin/Services/MyNewService.cs
```

Then modify the class name, logic, and config reference.

**Option B: From scratch**

Create `Services/MyNewService.cs`:
```csharp
using System;

public sealed class MyNewService : IDisposable
{
    private readonly Configuration _config;
    
    public MyNewService(Configuration config) 
    {
        _config = config;
    }
    
    public void Update()
    {
        // Called every frame
    }
    
    public void Dispose() { }
}
```

Then integrate in `Plugin.cs`:
```csharp
private MyNewService MyNewService { get; init; }

public Plugin()
{
    MyNewService = new MyNewService(Configuration);
    // ...
}

private void OnFrameworkUpdate(IFramework framework)
{
    MyNewService.Update();
}

public void Dispose()
{
    MyNewService?.Dispose();
    // ...
}
```

### Debug a Service

Add detailed logging:

```csharp
public void Update()
{
    _log.Debug($"Service running. Enabled={_config.MyFeature.Enabled}");
    
    if (!_config.MyFeature.Enabled)
    {
        _log.Debug("Feature disabled, skipping");
        return;
    }
    
    // Your logic here
    _log.Information($"Found {items.Count} items");
}
```

View in-game with `/xllog`.

### Modify UI

Files: `Windows/ConfigWindow.cs` and `Windows/MainWindow.cs`

Example adding a toggle:

```csharp
// In ConfigWindow.DrawConfigUI()
if (ImGui.Checkbox("Enable Treasure Hunting", 
    ref Plugin.Configuration.TreasureHunting.Enabled))
{
    Plugin.Configuration.Save();
    Plugin.Log?.Information("Treasure hunting toggled");
}
```

---

## File Navigation Guide

To understand what each file does:

| File | Purpose | Modify When |
|------|---------|-------------|
| `Configuration.cs` | Define settings | Adding new feature options |
| `Plugin.cs` | Coordinate services | Adding new service, hooks |
| `Services/TreasureHuntingService.cs` | Treasure logic | Changing treasure detection |
| `Services/CombatAutomationService.cs` | Combat logic | Changing combat behavior |
| `Windows/ConfigWindow.cs` | Settings UI | Redesigning settings display |
| `Windows/MainWindow.cs` | Status UI | Showing different info |
| `README.md` | Project info | Updating documentation |
| `ARCHITECTURE.md` | Design guide | Major refactoring |

---

## Debugging Checklist

Service not running?
- [ ] Is it initialized in `Plugin::Plugin()`?
- [ ] Is `Update()` called in `OnFrameworkUpdate()`?
- [ ] Is feature `Enabled` in config?
- [ ] Check `/xllog` for errors

Service not seeing objects?
- [ ] Is `ClientState.LocalPlayer` null? (not in-game)
- [ ] Is `ObjectTable` empty? (bad zone)
- [ ] Is object type correct? (searching for wrong type)

Config not persisting?
- [ ] Called `Configuration.Save()`?
- [ ] Is path writable? (`%APPDATA%\XIVLauncher\`)
- [ ] Check file size (should have content, not empty)

UI not updating?
- [ ] Is window `IsOpen`?
- [ ] Is data being passed from service?
- [ ] Check `ImGui.Text()` is being called

---

## IDE Setup (Optional but Recommended)

### Visual Studio 2022
- Install "Game Development with C++" workload
- Open `SamplePlugin.sln`
- Right-click project → "Set as Startup Project"
- F5 to build and debug

### Rider
- Open `SamplePlugin.sln`
- Run → Run (Shift+F10) to build

### VS Code
- Install C# extension (powered by OmniSharp)
- Open folder: `File → Open Folder → select oc-helper`
- Terminal: `dotnet build`

---

## Running Tests

Currently no automated tests, but you can manually test:

**Terrain Scan Test:**
```
1. Enable TreasureHunting
2. Go to Occult Crescent zone
3. Check /xllog for "Treasures found" messages
4. Move around, distance should update
```

**Safety Test:**
```
1. Set CombatAutomation.SafetyHpThreshold = 90
2. Enable AutoAttack
3. Let yourself take damage
4. At 90% HP, automation should STOP (check /xllog)
```

**Config Persistence Test:**
```
1. Change a setting in config window
2. Save (button or implicit on window close)
3. Unload plugin: /xlplugins disable OC-Helper
4. Reload plugin: /xlplugins enable OC-Helper
5. Setting should still be there
```

---

## Performance Tips

If the plugin is causing lag:

1. **Reduce Treasure Detection Radius**
   ```
   DetectionRadius: 100 → 50 (faster scanning)
   ```

2. **Reduce Max Active Loots**
   ```
   MaxActiveLoots: 50 → 10 (fewer objects tracked)
   ```

3. **Disable Unnecessary Features**
   ```
   TrackExperience: false (if not needed)
   DrawTreasureLines: false (less rendering)
   ```

4. **Add Early Returns**
   In services, check conditions early:
   ```csharp
   if (!_config.Feature.Enabled) return;  // Don't scan if disabled
   if (_clientState.LocalPlayer == null) return;
   ```

---

## Troubleshooting

### Build Error: "Type or namespace 'Xxx' not found"
- **Cause**: Missing using statement
- **Fix**: Right-click the type → Add using (VS/Rider IDE)

### Runtime Error: "NullReferenceException in Update()"
- **Cause**: Object is null when accessed
- **Fix**: Add null-check before using:
  ```csharp
  var player = _clientState.LocalPlayer;
  if (player == null) return;
  ```

### Plugin Won't Load
- **Check**: `/xllog` for startup errors
- **Common Causes**: 
  - Syntax errors in code (won't compile)
  - Plugin name conflicts
  - Damaged .json file

### Settings Don't Display
- **Check**: Is ImGui.Begin() called in ConfigWindow?
- **Common Causes**:
  - Window not being drawn
  - Incorrect ImGui syntax

---

## Next Steps

1. **Build successfully** →  Following "Step 1: Build"
2. **Deploy to game** → Follow "Step 2: Deploy"
3. **Test in-game** → Follow "Step 3: Load"
4. **Read documentation** → Check `ARCHITECTURE.md`
5. **Add a feature** → See "Add a New Setting"
6. **Deploy changes** → Rebuild and copy to plugin folder

---

## Getting Help

1. Check `ARCHITECTURE.md` for design patterns
2. Look at existing service as example
3. Search `/xllog` for error messages
4. Review XML documentation in source
5. Check `CONFIG_REFERENCE.md` for setting details

---

## Key Keyboard Shortcuts (IDE)

**Visual Studio 2022:**
- `F5` - Build and debug
- `Ctrl+Shift+B` - Build solution
- `Ctrl+F5` - Run without debug
- `Ctrl+K, Ctrl+D` - Format document

**Rider:**
- `Shift+F10` - Run
- `Ctrl+F9` - Build
- `Ctrl+Alt+L` - Reformat

**VS Code:**
- `Ctrl+Shift+B` - Run build task
- `Ctrl+F5` - Debug

---

## Useful Dalamud Documentation

- [Dalamud Docs](https://dalamud.dev/)
- [Dalamud API Reference](https://dalamud.dev/api/)
- [Plugin Development Guide](https://dalamud.dev/plugin-development/)

---

**You're ready to start developing!** 🚀

Next: Read `ARCHITECTURE.md` then start implementing `ConfigWindow.cs`
