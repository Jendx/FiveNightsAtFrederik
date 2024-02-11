using FiveNightsAtFrederik.Scenes.Player;
using Godot;
using System;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props;

/// <summary>
/// Spawns carryableItemTemplateScene instances when player moves the item far from spawn location 
/// </summary>
public partial class PickupSpawner : Node3D
{
    public delegate void ObjectLeftSpawnArear();
    public ObjectLeftSpawnArear OnObjectLeftSpawnArea;

    [Export]
    private bool initSpawn = true;

    [Export]
    private bool autoRespawn = true;

    [Export]
    private PackedScene carryableItemTemplateScene;
    
    private CarryableItem? currentItem;
    private bool isObjectAwayFromSpawn;

    public override void _Ready()
    {
        if (!initSpawn)
        {
            return;
        }

        currentItem = carryableItemTemplateScene.Instantiate() as CarryableItem;
        AddChild(currentItem);
    }

    public override void _Process(double delta)
    {
        isObjectAwayFromSpawn = GlobalPosition.DistanceTo(currentItem?.GlobalPosition) > 1f;
        if (autoRespawn)
        {
            SpawnItem();
        }
    }

    public void SpawnItem()
    {
        if (isObjectAwayFromSpawn)
        {
            GD.Print("Created new Item");
            currentItem = carryableItemTemplateScene.Instantiate() as CarryableItem;

            AddChild(currentItem);
        }
    }
}
