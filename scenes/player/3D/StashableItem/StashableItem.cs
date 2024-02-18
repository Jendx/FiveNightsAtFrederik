using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using System.Linq;

namespace FiveNightsAtFrederik.Scenes.Player;

[GlobalClass]
public partial class StashableItem : BaseCarriableItem, IStashable
{
	public bool IsStashed { get; set; }

	public void Stash()
	{
		Drop();
		IsStashed = true; 
	}

	protected override void Drop()
	{
		Reparent(originalParent);
		SetCollisionLayerValue((int)CollisionLayers.PlayerCollideable, true);
		base.Drop();
	}
}
