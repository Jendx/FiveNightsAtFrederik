using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;

namespace FiveNightsAtFrederik.Scenes.Player;

[GlobalClass]
public partial class Ingredient : BaseCarriableItem, IStashable
{
	[Export]
	public IngredientTypes Type { get; set; }

	/// <summary>
	/// Defines how ingredient will look when it is placed on pizza
	/// </summary>
	[Export]
	public Mesh PizzaDisplayMesh { get; set; }

	[Export]
	public int Count { get; set; } = 3;

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
