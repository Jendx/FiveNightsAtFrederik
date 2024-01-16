using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;

namespace FiveNightsAtFrederik.Scenes.Enemy;

/// <summary>
/// Mr Duck is enemy which should move like
/// </summary>
public partial class MrDuck : BaseEnemy, IMovableCharacter
{
    public float MovementSpeed { get; set; } = 1;
    public float JumpVelocity { get; set; }
    public float RotationSpeed { get; set; } = 0.5f;

    private readonly float MaximumTurnMoveAngle = 5;
    private Vector3 nextPosition;

    protected override void OnTargetReached()
    {
        CurrentMarker = controller.GetNextPossibleDestination();
        navigationAgent.TargetPosition = CurrentMarker.GlobalPosition;
        GD.Print($"Target Location switched to: {CurrentMarker.Name}");
    }

    protected override void Move(float delta)
    {
        nextPosition = navigationAgent.GetNextPathPosition();

        var direction = nextPosition.DirectionTo(GlobalPosition);
        var angle = direction.AngleTo(nextPosition.Normalized());

        if (angle > Mathf.DegToRad(MaximumTurnMoveAngle))
        {
            GD.Print($"Maximum turn angle reached {Mathf.RadToDeg(angle)} Current Angle: {MaximumTurnMoveAngle}");
            return;
        }
        
        Velocity = (nextPosition - GlobalPosition).Normalized() * MovementSpeed;

        GD.Print($"NextPosition: {nextPosition} Current Forward Vector {RotationDegrees.Y}");
        MoveAndSlide();
    }

    protected override void Rotate(float delta)
    {
        var targetVector = nextPosition.DirectionTo(GlobalPosition).Normalized();
        if (targetVector == Vector3.Zero) 
        {
            return;
        }

        var target = Basis.LookingAt(targetVector, Vector3.Up);
        Basis = Basis.Slerp(target, 0.1f);
        
    }
}
