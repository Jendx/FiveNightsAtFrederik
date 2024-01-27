using FiveNightsAtFrederik.Scenes.Player;
using Godot;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props;

/// <summary>
/// Spawns carryableItemTemplateScene instances when player moves the item far from spawn location 
/// </summary>
public partial class PickupSpawner : Node3D
{
    [Export]
    private PackedScene carryableItemTemplateScene;
    
    private CarryableItem currentItem;

    public override void _Ready()
    {
        currentItem = carryableItemTemplateScene.Instantiate() as CarryableItem;
        AddChild(currentItem);
    }

    public override void _Process(double delta)
    {
        if (GlobalPosition.DistanceTo(currentItem.GlobalPosition) > 1f)
        {
            GD.Print("Created new Item");
            currentItem = carryableItemTemplateScene.Instantiate() as CarryableItem;

            AddChild(currentItem);
        }
    }
}
