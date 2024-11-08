using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.scenes.player.Enums;
using Godot;
using System.Collections.Generic;

namespace FiveNightsAtFrederik.Scenes.Player;

/// <summary>
/// Basket holds n items
/// </summary>
public partial class Basket : BaseHoldableItem, IAnimated<PlayerAnimationStates?>
{
	[Export]
	private int maxCapacity = 3;

	private Area3D basketArea;
	private readonly List<IStashable> itemsInBasket;

    public Basket()
    {
        itemsInBasket = new List<IStashable>();
    }

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
		if(IsHeld)
		{
			return;
		}

		var item = body.TryConvertTo<Ingredient>();
		if (item is null || itemsInBasket.Contains(item) || itemsInBasket.Count >= maxCapacity)
		{
			return;
		}
		
		AddItemToBox(item);
	}

	public override void OnBeginUse()
	{
		if (player.IsCarryingItem)
		{
			return;
		}

		Freeze = true;
		Reparent(player.EquipableBasketPositionMarker);

		GlobalPosition = player.EquipableBasketPositionMarker.GlobalPosition;
		Rotation = Vector3.Zero;
		player.IsHoldingItem = true;
		IsHeld = true;
		SetCollisionLayerValue((int)CollisionLayers.PlayerCollideable, false);
	}

	private void AddItemToBox(Ingredient carriableItem)
	{
		itemsInBasket.Add(carriableItem);
		carriableItem.Stash();
		GD.Print(carriableItem.Name + "ENTERED");
		carriableItem.Freeze = true;
		carriableItem.Reparent(this);
		carriableItem.SetCollisionLayerValue((int)CollisionLayers.PlayerCollideable, false);
		carriableItem.OnItemPickedUp += BaseCarriableItem_ItemPickedUp;
	}

	private void BaseCarriableItem_ItemPickedUp(BaseCarriableItem item)
	{
		if (item is IStashable stashableItem)
		{
			item.OnItemPickedUp -= BaseCarriableItem_ItemPickedUp;
			itemsInBasket.Remove(stashableItem);
		}
	}

	public PlayerAnimationStates? HandleAnimations()
	{
		if (IsHeld)
		{
			return PlayerAnimationStates.Box;
		}

		return null;
	}
}
