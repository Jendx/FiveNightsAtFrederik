using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;
using System.Linq;

namespace FiveNightsAtFrederik.CsScripts.BaseNodes;
public partial class BaseCarryableItem : RigidBody3D, IPlayerUsable
{
    public bool IsHeld { get; set; }

    [Export]
    public bool IsInteractionUIDisplayed { get; set; } = true;

    [Export]
    public CollisionShape3D CollisionShape { get; set; }

    [Export]
    public MeshInstance3D MeshInstance { get; set; }

    private Vector3 direction;
    private Player player;
    private const float Speed = 0.001f;
    private const float MaxDistance = 1.5f;
    protected Node originalParent;

    public override void _Ready()
    {
        player = GetTree().GetNodesInGroup(GroupNames.playerGroup.ToString()).FirstOrDefault() as Player ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {NodeNames.PlayerInRoot}"); ;
        originalParent = GetParent();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsHeld)
        {
            return;
        }

        direction = (player.CarryableItemPositionMarker.GlobalPosition - GlobalPosition).Normalized();
        var distance = GlobalPosition.DistanceTo(player.CarryableItemPositionMarker.GlobalPosition);

        // To prevent "Lagging" from trying to move closer than it is possible (big Speed)
        if (distance < 0.01f)
        {
            return;
        }

        // If player is too far away from the object, stop holding object
        if (distance > MaxDistance)
        {
            Drop();
        }

        var motion = direction * (Speed + distance);
        MoveAndCollide(motion);
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustReleased(ActionNames.Use) && IsHeld)
        {
            Drop();
        }
    }

    private void Drop()
    {
        Freeze = false;
        IsHeld = false;

        ApplyCentralImpulse(direction);
    }

    public void OnBeginUse()
    {
        IsHeld = true;
        Freeze = true;
        Reparent(originalParent);
        SetCollisionLayerValue((int)CollisionLayers.PlayerCollideable, true);
    }

    public void OnEndUse()
    {
        if (!Input.IsActionJustReleased(ActionNames.Use))
        {
            return;
        }

        Drop();
    }
}