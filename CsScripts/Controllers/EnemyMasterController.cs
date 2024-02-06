using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using FiveNightsAtFrederik.Scenes.Enemy;
using FiveNightsAtFrederik.CsScripts.Models;
using FiveNightsAtFrederik.CsScripts.BaseNodes;

namespace FiveNightsAtFrederik.CsScripts.Controllers;

public class EnemyMasterController
{
	private static Dictionary<StringName, LocationMarkers> markers;
	private BaseEnemy baseEnemy;
	private readonly Random random = new(1);

	public EnemyMasterController(BaseEnemy baseEnemy)
	{
		this.baseEnemy = baseEnemy;
		var siblings = this.baseEnemy.GetParent().GetChildren();
		markers ??= siblings
			.Where(s => s is Marker3D && s.Name.ToString().Contains("E_"))
			.Cast<Marker3D>()
            .ToDictionary(s => s.Name, s => new LocationMarkers()
            {
                IsOccupied = false,
                Marker = s
            });
	}

    /// <summary>
    /// Resets the used markers so they can be picked as next location
    /// </summary>
    public static void ResetUsedMarkers(string enemyName)
    {
        foreach (var marker in markers.Where(m => m.Value.OccupiedBy == enemyName).ToArray())
        {
            marker.Value.IsOccupied = false;
        }
    }

	public Marker3D GetNextPossibleDestination(string enemyName)
	{
		var availableTargetMarkers = markers.Where(m => !m.Value.IsOccupied).ToArray();
		var nextMarker = availableTargetMarkers[random.Next(availableTargetMarkers.Length)];
        markers[nextMarker.Key].IsOccupied = true;
        markers[nextMarker.Key].OccupiedBy = enemyName;

		// Make current Marker visit-able again
		if(markers.TryGetValue(baseEnemy?.CurrentDestinationMarker?.Name ?? string.Empty, out var value) && value.OccupiedBy == enemyName)
		{
			markers[baseEnemy.CurrentDestinationMarker.Name].IsOccupied = false;
			GD.Print($"Resetting {baseEnemy.CurrentDestinationMarker.Name}");
		}

		return nextMarker.Value.Marker;
	}
}
