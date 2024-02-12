using FiveNightsAtFrederik.Scenes.Player;
using Godot;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props;

/// <summary>
/// Spawns carryableItemTemplateScene instances when player moves the item far from spawn location 
/// </summary>
public partial class PickupSpawner : Node3D
{
    public delegate void ObjectLeftSpawnArear();
    public event ObjectLeftSpawnArear? OnObjectLeftSpawnArea;

    /// <summary>
    /// Determines if spawner should spawn item, when game starts
    /// </summary>
    [Export]
    private bool spawnOnReady = true;

    [Export]
    private bool autoRespawn = true;

    [Export]
    private PackedScene? carryableItemTemplateScene;
    
    private CarryableItem? currentItem;
    private bool isObjectAwayFromSpawn;

    public override void _Ready()
    {
        carryableItemTemplateScene = carryableItemTemplateScene ?? throw new System.Exception($"Node: {Name} does not have {nameof(carryableItemTemplateScene)} set!");
        if (!spawnOnReady)
        {
            return;
        }

        currentItem = carryableItemTemplateScene?.Instantiate() as CarryableItem;
        AddChild(currentItem);
    }

    public override void _Process(double delta)
    {
        isObjectAwayFromSpawn = GlobalPosition.DistanceTo(currentItem?.GlobalPosition ?? GlobalPosition * 3) > 1f;

        if (isObjectAwayFromSpawn && currentItem is not null)
        {
            currentItem = null;
            OnObjectLeftSpawnArea?.Invoke();
        }

        if (autoRespawn)
        {
            TrySpawnItem();
        }
    }

    public CarryableItem? TrySpawnItem()
    {
        if (isObjectAwayFromSpawn)
        {
            currentItem = carryableItemTemplateScene?.Instantiate() as CarryableItem;

            AddChild(currentItem);
        }

        return currentItem;
    }
}
