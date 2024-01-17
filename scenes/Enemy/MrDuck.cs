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

        // Calculates vector with direction from GlobalPosition to nextPosition
        var nextPositionDirection = nextPosition.DirectionTo(GlobalPosition);

        // Calculates vector with direction from LookForwardPosition to GlobalPosition So I do not have to figure out how to get forward vector in Global Cords 
        var forwardDirection = LookForwardPosition.GlobalPosition.DirectionTo(GlobalPosition);
        var AngleBetweenLookingAndNextPosition = forwardDirection.AngleTo(nextPositionDirection);

        if (AngleBetweenLookingAndNextPosition > Mathf.DegToRad(MaximumTurnMoveAngle))
        {
            GD.Print($"Maximum turn angle reached ({MaximumTurnMoveAngle}) Current Angle: {Mathf.RadToDeg(AngleBetweenLookingAndNextPosition)}");
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

        var target = Basis.LookingAt(targetVector, Vector3.Up).Orthonormalized();
        Basis = Basis.Slerp(target, 0.1f);
        
    }
}
