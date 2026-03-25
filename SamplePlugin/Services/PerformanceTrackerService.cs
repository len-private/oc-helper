using System;
using Dalamud.Plugin.Services;

namespace SamplePlugin.Services;

/// <summary>
/// Service for tracking farming performance metrics.
/// 
/// Responsibilities:
/// - Track gil earned during session
/// - Track experience gained
/// - Calculate rates (gil/hour, exp/hour)
/// - Reset stats on schedule
/// </summary>
public sealed class PerformanceTrackerService : IDisposable
{
    private readonly Configuration _config;
    private readonly IPluginLog _log;

    // Session statistics
    private uint _sessionGilEarned;
    private ulong _sessionExpGained;
    private DateTime _sessionStartTime;
    private uint _lastRecordedGil;
    private ulong _lastRecordedExp;

    public uint GilPerHour { get; private set; }
    public ulong ExpPerHour { get; private set; }

    public PerformanceTrackerService(Configuration config, IPluginLog log)
    {
        _config = config;
        _log = log;
        ResetSession();
    }

    /// <summary>
    /// Initialize a new session.
    /// </summary>
    public void ResetSession()
    {
        _sessionGilEarned = 0;
        _sessionExpGained = 0;
        _sessionStartTime = DateTime.Now;
        _lastRecordedGil = 0;
        _lastRecordedExp = 0;
        GilPerHour = 0;
        ExpPerHour = 0;
        _log?.Information("Performance tracker session reset");
    }

    /// <summary>
    /// Record gil earned. Call this when loot is collected.
    /// </summary>
    public void RecordGilEarned(uint amount)
    {
        if (!_config.PerformanceTracker.TrackGil)
            return;

        _sessionGilEarned += amount;
        UpdateRates();
    }

    /// <summary>
    /// Record experience gained. Call this when XP is earned.
    /// </summary>
    public void RecordExpGained(ulong amount)
    {
        if (!_config.PerformanceTracker.TrackExperience)
            return;

        _sessionExpGained += amount;
        UpdateRates();
    }

    /// <summary>
    /// Update hourly rates based on elapsed time.
    /// </summary>
    private void UpdateRates()
    {
        var elapsed = DateTime.Now - _sessionStartTime;
        if (elapsed.TotalSeconds < 1)
            return;

        var hoursElapsed = elapsed.TotalHours;
        if (hoursElapsed > 0)
        {
            GilPerHour = (uint)(_sessionGilEarned / hoursElapsed);
            ExpPerHour = (ulong)(_sessionExpGained / hoursElapsed);
        }
    }

    /// <summary>
    /// Get formatted session statistics.
    /// </summary>
    public string GetSessionSummary()
    {
        var elapsed = DateTime.Now - _sessionStartTime;
        return $"Gil: {_sessionGilEarned} ({GilPerHour}/hr) | " +
               $"Exp: {_sessionExpGained} ({ExpPerHour}/hr) | " +
               $"Time: {elapsed:hh\\:mm\\:ss}";
    }

    /// <summary>
    /// Check if auto-reset time has been reached and reset if needed.
    /// </summary>
    public void CheckAutoReset()
    {
        if (_config.PerformanceTracker.AutoResetHour < 0)
            return;

        if (DateTime.Now.Hour == _config.PerformanceTracker.AutoResetHour &&
            (DateTime.Now - _sessionStartTime).TotalHours >= 1)
        {
            ResetSession();
        }
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
