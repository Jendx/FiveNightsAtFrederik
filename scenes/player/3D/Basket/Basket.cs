using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using System.Collections.Generic;

namespace FiveNightsAtFrederik.Scenes.Player;

/// <summary>
/// Basket holds n items
/// </summary>
public partial class Basket : BaseHoldableItem
{
	[Export]
	private const int maxCapacity = 3;

	private Area3D basketArea;
	private List<IStashable> itemsInBasket = new(maxCapacity);

	public override void _Ready()
	{
		basketArea = this.TryGetNode<Area3D>(NodeNames.BasketArea, nameof(basketArea));
		base._Ready();
		basketArea.BodyEntered += BasketArea_BodyEntered;
	}

   /// <summary>
   /// Checks whenever Node3D body satisfies conditions and if so then it parents it to the basket
   /// </summary>
   /// <param name="body"></param>
	private void BasketArea_BodyEntered(Node3D body)
	{
		var item = body.TryConvertTo<IStashable>();
		if (item is null || itemsInBasket.Contains(item) || itemsInBasket.Count >= maxCapacity)
		{
			return;
		}
		
		AddItemToBox((BaseCarriableItem)item);
	}

	private void AddItemToBox(BaseCarriableItem body)
	{
		itemsInBasket.Add((IStashable)body);

		GD.Print(body.Name + "ENTERED");
		body.Freeze = true;
		body.Reparent(this);
		body.SetCollisionLayerValue((int)CollisionLayers.PlayerCollideable, false);
	}
}
