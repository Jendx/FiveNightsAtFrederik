using FiveNightsAtFrederik.Scenes.Player;
using Godot;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props;

public partial class PickupSpawner : Node3D
{
    [Export]
    private PackedScene carryableItemTemplate;
    
    private CarryableItem currentItem;

    public override void _Ready()
    {
        currentItem = carryableItemTemplate.Instantiate() as CarryableItem;
        AddChild(currentItem);
    }

    public override void _Process(double delta)
    {
        if (GlobalPosition.DistanceTo(currentItem.GlobalPosition) > 1f)
        {
            GD.Print("Created new Item");
            currentItem = carryableItemTemplate.Instantiate() as CarryableItem;

            AddChild(currentItem);
        }
    }
}
