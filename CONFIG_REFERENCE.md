# OC-Helper Configuration Reference

This document explains every configuration option in OC-Helper and its effects.

## Where Settings Are Stored

Settings are automatically saved to and loaded from:
```
%APPDATA%\XIVLauncher\pluginConfigs\OC-Helper\SamplePlugin.json
```

Changes are synced to disk when you call `Configuration.Save()`.

---

## General Settings

### `IsConfigWindowMovable`
- **Type:** Boolean
- **Default:** `true`
- **Effect:** Whether the configuration window can be moved by dragging its title bar
- **Use case:** Disable if you want to keep the config window in a fixed position

---

## Treasure Hunting Configuration

Located at: `Configuration.TreasureHunting`

### `Enabled`
- **Type:** Boolean
- **Default:** `false`
- **Effect:** Master toggle for treasure hunting automation
- **Impact when true:** Treasure objects are scanned every frame, positions are tracked
- **Impact when false:** No treasure scanning occurs (negligible performance cost)

### `DetectionRadius`
- **Type:** Float (yalms)
- **Default:** `100f`
- **Range:** 0.1 - 500
- **Effect:** Distance (in yalms) at which the plugin detects treasures
- **Notes:**
  - Larger radius = more treasures tracked but higher CPU cost
  - ~100 is typical for a zone view
  - Increase if you're missing distant treasures

**Example:**
```
DetectionRadius: 100  → Sees treasures up to 100 yalms away
DetectionRadius: 50   → Only sees treasures within 50 yalms
```

### `AutoPathfind`
- **Type:** Boolean
- **Default:** `true`
- **Effect:** Enable automatic navigation to the nearest treasure
- **Implementation:** Currently disabled (TODO: integrate pathfinding library)
- **When functional:** Will automatically move player toward nearest treasure

### `DrawTreasureLines`
- **Type:** Boolean
- **Default:** `true`
- **Effect:** Draw a visual line from player to nearby treasures (world overlay)
- **Use case:** Visual reference for where treasures are
- **Notes:**
  - Only draws to treasures within `DetectionRadius`
  - Helps with manual hunting without automation

### `MaxActiveLoots`
- **Type:** Integer
- **Default:** `10`
- **Range:** 1 - 1000
- **Effect:** Maximum number of treasures tracked simultaneously
- **Use case:** Limit processing cost or UI clutter
- **Behavior:** If more than this many treasures are detected, only the closest ones are tracked

**Example:**
```
MaxActiveLoots: 10   → Tracks 10 closest treasures
MaxActiveLoots: 1    → Tracks only the nearest treasure
MaxActiveLoots: 100  → Tracks up to 100 treasures (high cost)
```

---

## Combat Automation Configuration

Located at: `Configuration.CombatAutomation`

### `Enabled`
- **Type:** Boolean
- **Default:** `false`
- **Effect:** Master toggle for combat automation
- **Impact when true:** Combat logic runs every frame, enemy scanning is active
- **Impact when false:** Combat system inactive

### `AutoAttack`
- **Type:** Boolean
- **Default:** `false`
- **Effect:** Automatically attack enemies within `CombatRange`
- **Behavior:**
  - Finds the nearest enemy within range
  - Initiates combat attack
  - Continues attacking until enemy dies or out of range
- **Safety:** Respects `SafetyHpThreshold` — won't attack if low on HP

**Example:**
```
AutoAttack: true   → Plugin attacks first enemy it sees
AutoAttack: false  → Manual combat only
```

### `UseAutoBuffs`
- **Type:** Boolean
- **Default:** `false`
- **Effect:** Cast support buffs automatically (Bard/Knight/Monk)
- **Implementation:** Currently disabled (TODO: implement buff rotation)
- **Prerequisites:** Requires proper job setup in config (not yet available)
- **When functional:** Will cast job-specific buffs to improve farming efficiency

### `CombatRange`
- **Type:** Float (yalms)
- **Default:** `25f`
- **Range:** 1 - 100
- **Effect:** Distance at which enemies are considered "in range"
- **Use case:** How far to look for enemies to attack
- **Notes:**
  - Most melee weapons have ~2.5-3 yalm range
  - Set this larger to engage from distance
  - Set it smaller to only attack very close enemies

**Range reference:**
- Melee: 2.5-3 yalms (sword, axe, spear)
- Ranged DPS: ~25 yalms (bow, gun)
- Caster: ~20-25 yalms (staff, wand)

### `SafetyHpThreshold`
- **Type:** Integer (percent)
- **Default:** `30`
- **Range:** 1 - 100
- **Effect:** Disable automation if player HP drops below this percentage
- **Behavior:**
  - Example: `SafetyHpThreshold: 30` means "pause if HP < 30%"
  - When triggered: Sets `CombatAutomation.Enabled = false`
  - Player can manually re-enable after healing
- **Safety:** Prevents deaths from automation running blindly

**Examples:**
```
SafetyHpThreshold: 30  → Pause at 30% HP (aggressive)
SafetyHpThreshold: 50  → Pause at 50% HP (safe)
SafetyHpThreshold: 10  → Pause at 10% HP (risky)
```

---

## Performance Tracker Configuration

Located at: `Configuration.PerformanceTracker`

### `Enabled`
- **Type:** Boolean
- **Default:** `false`
- **Effect:** Master toggle for performance metric tracking
- **Impact when true:** Tracks all enabled metrics during farming
- **Impact when false:** No stats are collected

### `TrackGil`
- **Type:** Boolean
- **Default:** `true`
- **Effect:** Record gil earned during session
- **Calculation:**
  - Displays total gil earned
  - Calculates gil per hour
  - Example: "5000 gil (5000/hr after 1 hour)"

### `TrackExperience`
- **Type:** Boolean
- **Default:** `true`
- **Effect:** Record experience earned during session
- **Calculation:**
  - Displays total exp gained
  - Calculates exp per hour
  - Example: "50000 exp (50000/hr after 1 hour)"

### `AutoResetHour`
- **Type:** Integer (0-23, or -1 to disable)
- **Default:** `-1` (disabled)
- **Effect:** Automatically reset statistics at specified hour
- **Use case:** Daily farming reports, session analysis
- **Behavior:**
  - At the specified hour, stats reset to zero
  - Very first reset only happens after full hour elapse
  - Prevents false stat resets

**Examples:**
```
AutoResetHour: -1   → No automatic reset (manual reset only)
AutoResetHour: 0    → Reset at midnight (00:00)
AutoResetHour: 6    → Reset at 6 AM (06:00)
AutoResetHour: 18   → Reset at 6 PM (18:00)
```

---

## Editing Configuration Manually

You can edit `SamplePlugin.json` directly if needed:

```json
{
  "Version": 1,
  "IsConfigWindowMovable": true,
  "TreasureHunting": {
    "Enabled": true,
    "DetectionRadius": 100.0,
    "AutoPathfind": true,
    "DrawTreasureLines": true,
    "MaxActiveLoots": 10
  },
  "CombatAutomation": {
    "Enabled": false,
    "AutoAttack": false,
    "UseAutoBuffs": false,
    "CombatRange": 25.0,
    "SafetyHpThreshold": 30
  },
  "PerformanceTracker": {
    "Enabled": true,
    "TrackGil": true,
    "TrackExperience": true,
    "AutoResetHour": -1
  }
}
```

⚠️ **Warning:** 
- Close the plugin before editing the JSON manually
- Invalid JSON will cause the file to fail loading
- If corrupted, delete the file—defaults will recreate it

---

## Configuration Best Practices

### Safe Farming Setup
```json
{
  "TreasureHunting": {
    "Enabled": true,
    "DetectionRadius": 100,
    "AutoPathfind": false,
    "MaxActiveLoots": 10
  },
  "CombatAutomation": {
    "Enabled": false,
    "SafetyHpThreshold": 50
  },
  "PerformanceTracker": {
    "Enabled": true
  }
}
```

### Aggressive Farming (Higher Risk)
```json
{
  "TreasureHunting": {
    "Enabled": true,
    "DetectionRadius": 150,
    "AutoPathfind": true,
    "MaxActiveLoots": 50
  },
  "CombatAutomation": {
    "Enabled": true,
    "AutoAttack": true,
    "SafetyHpThreshold": 20
  },
  "PerformanceTracker": {
    "Enabled": true
  }
}
```

### Statistics-Only Setup
```json
{
  "TreasureHunting": {
    "Enabled": false
  },
  "CombatAutomation": {
    "Enabled": false
  },
  "PerformanceTracker": {
    "Enabled": true,
    "AutoResetHour": 0
  }
}
```

---

## Troubleshooting Configuration

### Settings not saving?
- Ensure you call `Configuration.Save()` after changes
- Check that the config file isn't read-only
- Look for errors in `/xllog`

### Settings reset to defaults?
- The JSON file corrupted (delete and recreate)
- Version mismatch (update to latest plugin version)
- Permissions issue (ensure Dalamud can write to config directory)

### Performance issues?
- Reduce `DetectionRadius` (too many treasures being tracked)
- Reduce `MaxActiveLoots`
- Disable `DrawTreasureLines`
- Disable `TrackGil` or `TrackExperience` if not needed

### Automation not working?
- Is the feature `Enabled`? Check in config window
- Check `/xllog` for log messages
- Verify you're in Occult Crescent
- Ensure you're logged in (`ClientState.LocalPlayer` must not be null)

---

## Default Factory Reset

To reset all settings to defaults:

1. Stop the plugin: `/xlplugins disable OC-Helper`
2. Delete: `%APPDATA%\XIVLauncher\pluginConfigs\OC-Helper\`
3. Restart the plugin: `/xlplugins enable OC-Helper`

Settings will be recreated with all defaults.

---

## Configuration Change Checklist

When modifying config in-game:

- [ ] Toggle feature `Enabled` to see immediate effect
- [ ] Adjust numerical parameters gradually (e.g., increase `DetectionRadius` by 25)
- [ ] Watch `/xllog` for errors or warnings
- [ ] Monitor performance (FPS should not drop)
- [ ] Test feature in a safe location first
- [ ] Save with `Configuration.Save()` before closing the config window

---

## See Also

- `ARCHITECTURE.md` - How the config system works internally
- `Configuration.cs` - Source code for all config classes
- `README.md` - General plugin information
