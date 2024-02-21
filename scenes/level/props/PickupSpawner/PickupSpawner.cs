using FiveNightsAtFrederik.CsScripts.BaseNodes;
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
    private PackedScene? carriableItemTemplateScene;
    
    private BaseCarriableItem? currentItem;
    private bool isObjectAwayFromSpawn;

    public override void _Ready()
    {
        carriableItemTemplateScene = carriableItemTemplateScene ?? throw new System.Exception($"Node: {Name} does not have {nameof(carriableItemTemplateScene)} set!");
        if (!spawnOnReady)
        {
            return;
        }

        currentItem = carriableItemTemplateScene?.Instantiate() as BaseCarriableItem;
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
            TrySpawnItem<CarriableItem>();
        }
    }

    public TItem? TrySpawnItem<TItem>() where TItem : BaseCarriableItem
    {
        if (isObjectAwayFromSpawn)
        {
            currentItem = carriableItemTemplateScene?.Instantiate() as TItem;
            if (currentItem is null)
            {
                GD.PrintErr($"PickupSpawner: {Name} failed to spawn item of type {nameof(TItem)}");
            }

            AddChild(currentItem);
        }

        return (TItem?)currentItem;
    }
}
