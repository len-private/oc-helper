using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;

namespace SamplePlugin.Services;

/// <summary>
/// Service for automating combat in Occult Crescent.
/// 
/// Responsibilities:
/// - Detect nearby enemies
/// - Manage attack automation
/// - Apply support buffs when configured
/// - Monitor player safety (HP threshold)
/// </summary>
public sealed class CombatAutomationService : IDisposable
{
    private readonly Configuration _config;
    private readonly IObjectTable _objectTable;
    private readonly IClientState _clientState;
    private readonly IPluginLog _log;

    public CombatAutomationService(Configuration config, IObjectTable objectTable, IClientState clientState, IPluginLog log)
    {
        _config = config;
        _objectTable = objectTable;
        _clientState = clientState;
        _log = log;
    }

    /// <summary>
    /// Update combat automation state. Call once per frame.
    /// </summary>
    public void Update()
    {
        if (!_config.CombatAutomation.Enabled)
            return;

        var player = _clientState.LocalPlayer;
        if (player == null)
            return;

        // Safety check: don't engage if HP is too low
        if (player.CurrentHp < (player.MaxHp * _config.CombatAutomation.SafetyHpThreshold / 100f))
        {
            OnSafetyThresholdBreach(player);
            return;
        }

        if (_config.CombatAutomation.AutoAttack)
        {
            HandleAutoAttack(player);
        }

        if (_config.CombatAutomation.UseAutoBuffs)
        {
            HandleAutoBuffs(player);
        }
    }

    /// <summary>
    /// Find and attack nearby enemies.
    /// </summary>
    private void HandleAutoAttack(PlayerCharacter player)
    {
        var nearestEnemy = FindNearestEnemy(player);
        if (nearestEnemy == null)
            return;

        // TODO: Implement actual attack command
        // This would typically use game command execution or TargetingService
        Plugin.Log?.Information($"Auto-attacking: {nearestEnemy.Name}");
    }

    /// <summary>
    /// Apply support buffs based on current job.
    /// </summary>
    private void HandleAutoBuffs(PlayerCharacter player)
    {
        // TODO: Detect job and apply appropriate buffs
        // Examples:
        // - Bard: Army's Paeon, etc.
        // - Knight: Defiance for dungeons
        // - Monk: Form rotation setup
        
        Plugin.Log?.Debug("Auto-buff handler called");
    }

    /// <summary>
    /// Called when player HP drops below safety threshold.
    /// Override this for custom behavior (pause, use heal, etc).
    /// </summary>
    private void OnSafetyThresholdBreach(PlayerCharacter player)
    {
        Plugin.Log?.Warning($"Safety threshold breached! HP: {player.CurrentHp}/{player.MaxHp}");
        // Disable automation until manual re-enable
        _config.CombatAutomation.Enabled = false;
        _config.Save();
    }

    /// <summary>
    /// Find the nearest combatable enemy within range.
    /// </summary>
    private GameObject? FindNearestEnemy(PlayerCharacter player)
    {
        GameObject? nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (var obj in _objectTable)
        {
            if (obj is not BattleNpc enemy)
                continue;

            // Skip friendlies and dead enemies
            if (enemy.IsFriendly || enemy.IsDead)
                continue;

            var distance = Vector3.Distance(player.Position, obj.Position);
            if (distance > _config.CombatAutomation.CombatRange)
                continue;

            if (distance < nearestDistance)
            {
                nearest = obj;
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
