using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;

namespace SamplePlugin.Services;

/// <summary>
/// Service for automating treasure hunting in Occult Crescent.
/// 
/// Responsibilities:
/// - Detect treasure objects in the world
/// - Track nearby treasures and their positions
/// - Provide pathfinding targets
/// - Filter treasures based on configuration
/// </summary>
public sealed class TreasureHuntingService : IDisposable
{
    private readonly Configuration _config;
    private readonly IObjectTable _objectTable;
    private readonly IClientState _clientState;
    private readonly IPluginLog _log;

    /// <summary>
    /// Current list of detected treasures (sorted by distance).
    /// </summary>
    public IReadOnlyList<TreasureNode> NearbyTreasures { get; private set; } = new List<TreasureNode>();

    public TreasureHuntingService(Configuration config, IObjectTable objectTable, IClientState clientState, IPluginLog log)
    {
        _config = config;
        _objectTable = objectTable;
        _clientState = clientState;
        _log = log;
    }

    /// <summary>
    /// Scan the world for treasure objects and update the nearby treasures list.
    /// </summary>
    public void Update()
    {
        if (!_config.TreasureHunting.Enabled || _clientState.LocalPlayer == null)
        {
            NearbyTreasures = new List<TreasureNode>();
            return;
        }

        var playerPos = _clientState.LocalPlayer.Position;
        var treasures = new List<TreasureNode>();

        // Scan all objects in the object table for treasure nodes
        // Treasure objects typically have specific object kinds or data IDs
        foreach (var obj in _objectTable)
        {
            if (obj is not GameObject { ObjectKind: ObjectKind.Treasure })
                continue;

            var distance = Vector3.Distance(playerPos, obj.Position);
            if (distance > _config.TreasureHunting.DetectionRadius)
                continue;

            treasures.Add(new TreasureNode
            {
                GameObjectId = obj.GameObjectId,
                Position = obj.Position,
                Distance = distance,
                DataId = obj.DataId,
            });
        }

        // Sort by distance and limit to max active loots
        NearbyTreasures = treasures
            .OrderBy(t => t.Distance)
            .Take(_config.TreasureHunting.MaxActiveLoots)
            .ToList();
    }

    /// <summary>
    /// Get the closest treasure to the player.
    /// </summary>
    public TreasureNode? GetClosestTreasure() => NearbyTreasures.FirstOrDefault();

    /// <summary>
    /// Interaction target for pathfinding (closest treasure position).
    /// </summary>
    public Vector3? GetPathfindingTarget() => GetClosestTreasure()?.Position;

    public void Dispose()
    {
        // Cleanup if needed
    }
}

/// <summary>
/// Represents a treasure node in the world.
/// </summary>
public class TreasureNode
{
    public uint GameObjectId { get; set; }
    public Vector3 Position { get; set; }
    public float Distance { get; set; }
    public uint DataId { get; set; }
}
