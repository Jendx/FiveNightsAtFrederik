using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using FiveNightsAtFrederik.Scenes.Enemy;

namespace FiveNightsAtFrederik.CsScripts.Controllers;

public class EnemyMasterController
{
	private static Dictionary<StringName, (bool, Marker3D)> markers;
	private BaseEnemy baseEnemy;
	private readonly Random random = new(1);

	public EnemyMasterController(BaseEnemy baseEnemy)
	{
		this.baseEnemy = baseEnemy;
		var siblings = this.baseEnemy.GetParent().GetChildren();
		markers ??= siblings
			.Where(s => s is Marker3D && s.Name.ToString().Contains("E_"))
			.Cast<Marker3D>().ToDictionary(s => s.Name, s => (false, s));
	}

	public Marker3D GetNextPossibleDestination()
	{
		var availableTargetMarkers = markers.Where(m => !m.Value.Item1).ToArray();
		var nextMarker = availableTargetMarkers[random.Next(availableTargetMarkers.Length)];
		markers[nextMarker.Key] = (true, markers[nextMarker.Key].Item2);

		// Make current Marker visitable again
		if(markers.TryGetValue(baseEnemy?.CurrentDestinationMarker?.Name ?? string.Empty, out var value))
		{
			markers[baseEnemy.CurrentDestinationMarker.Name] = (false, value.Item2);
			GD.Print($"Reseting {baseEnemy.CurrentDestinationMarker.Name}");
		}

		return nextMarker.Value.Item2;
	}
}
