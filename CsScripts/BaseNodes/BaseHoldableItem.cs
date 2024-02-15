using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;
using System.Linq;

namespace FiveNightsAtFrederik.CsScripts.BaseNodes;

public partial class BaseHoldableItem : RigidBody3D, IPlayerUsable
{
    [Export]
    public bool IsInteractionUIDisplayed { get; set; } = true;

    [Export]
    public bool IsHeld { get; set; }

    protected Player player;
    protected Node originalParent;

    public override void _Ready()
    {
        player = GetTree().GetNodesInGroup(GroupNames.playerGroup.ToString()).FirstOrDefault() as Player ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {NodeNames.PlayerInRoot}");
        originalParent = GetParent();
    }

    public override void _Input(InputEvent @event)
    {
         Drop();
    }

    /// <summary>
    /// Unparents carryable item from player
    /// </summary>
    protected virtual void Drop()
    {
        if (!(IsHeld && Input.IsActionJustPressed(ActionNames.Drop)))
        {
            return;
        }

        Reparent(originalParent);
        player.IsHoldingItem = false;
        IsHeld = false;
        Freeze = false;
        SetCollisionLayerValue((int)CollisionLayers.PlayerCollideable, true);
    }

    /// <summary>
    /// Attaches item to player & makes it not collidible with player
    /// </summary>
    public virtual void OnBeginUse()
    {
        if (player.IsCarryingItem)
        {
            return;
        }

        Freeze = true;
        Reparent(player.EquipableItemPositionMarker);

        GlobalPosition = player.EquipableItemPositionMarker.GlobalPosition;
        Rotation = player.EquipableItemPositionMarker.Rotation;

        player.IsHoldingItem = true;
        IsHeld = true;
        SetCollisionLayerValue((int)CollisionLayers.PlayerCollideable, false);
    }

    public virtual void OnEndUse() { }
}
