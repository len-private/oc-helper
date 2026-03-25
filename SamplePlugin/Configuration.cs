using Dalamud.Configuration;
using System;

namespace SamplePlugin;

/// <summary>
/// Main plugin configuration. All properties here are automatically serialized.
/// Each section corresponds to a feature module.
/// </summary>
[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    // UI Settings
    public bool IsConfigWindowMovable { get; set; } = true;

    // Feature Toggles
    public TreasureHuntingConfig TreasureHunting { get; set; } = new();
    public CombatAutomationConfig CombatAutomation { get; set; } = new();
    public PerformanceTrackerConfig PerformanceTracker { get; set; } = new();

    /// <summary>
    /// Save configuration to disk. Call this after making changes.
    /// </summary>
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}

/// <summary>
/// Configuration for treasure hunting automation.
/// Controls what treasures to hunt and how to handle them.
/// </summary>
[Serializable]
public class TreasureHuntingConfig
{
    /// <summary>Enable/disable treasure hunting automation</summary>
    public bool Enabled { get; set; } = false;

    /// <summary>Distance (yalms) to detect treasures</summary>
    public float DetectionRadius { get; set; } = 100f;

    /// <summary>Automatically navigate to nearest treasure</summary>
    public bool AutoPathfind { get; set; } = true;

    /// <summary>Draw visual line to nearby treasures</summary>
    public bool DrawTreasureLines { get; set; } = true;

    /// <summary>Maximum treasures to track simultaneously</summary>
    public int MaxActiveLoots { get; set; } = 10;
}

/// <summary>
/// Configuration for combat automation.
/// Controls combat behavior and farming preferences.
/// </summary>
[Serializable]
public class CombatAutomationConfig
{
    /// <summary>Enable/disable combat automation</summary>
    public bool Enabled { get; set; } = false;

    /// <summary>Automatically engage enemies within range</summary>
    public bool AutoAttack { get; set; } = false;

    /// <summary>Cast support buffs automatically (requires job setup)</summary>
    public bool UseAutoBuffs { get; set; } = false;

    /// <summary>Combat range (yalms)</summary>
    public float CombatRange { get; set; } = 25f;

    /// <summary>Pause automation if player HP drops below this percent</summary>
    public int SafetyHpThreshold { get; set; } = 30;
}

/// <summary>
/// Configuration for performance metrics tracking.
/// Tracks farming yields and efficiency.
/// </summary>
[Serializable]
public class PerformanceTrackerConfig
{
    /// <summary>Enable performance metric tracking</summary>
    public bool Enabled { get; set; } = false;

    /// <summary>Track gil earned per session</summary>
    public bool TrackGil { get; set; } = true;

    /// <summary>Track experience gained per session</summary>
    public bool TrackExperience { get; set; } = true;

    /// <summary>Reset stats at this hour (0-23, -1 to disable auto-reset)</summary>
    public int AutoResetHour { get; set; } = -1;
}
