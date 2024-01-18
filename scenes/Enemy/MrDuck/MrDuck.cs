using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.scenes.Enemy.MrDuck;
using Godot;
using System;
using System.ComponentModel;

namespace FiveNightsAtFrederik.Scenes.Enemy;

/// <summary>
/// Mr Duck is enemy which should move like
/// </summary>
public partial class MrDuck : BaseEnemy, IMovableCharacter
{
    public float MovementSpeed { get; set; } = 1;
    public float JumpVelocity { get; set; }
    public float RotationSpeed { get; set; } = 0.01f;

    private readonly float MaximumTurnMoveAngle = 5;
    private AnimationTree animationTree;
    private MrDuckAnimations currentAnimations = MrDuckAnimations.Idle;

    /// <summary>
    /// Slowly interpolates between ForwardPosition and nextPosition, which allows for smoot rotation 
    /// </summary>
    private Vector3 interpolationCurrentPosition;
    private Vector3 nextPosition;

    public override void _Ready()
    {
        base._Ready();
        interpolationCurrentPosition = LookForwardPosition.GlobalPosition;

        animationTree = GetNode<AnimationTree>(NodeNames.MrDuckAnimationTree.ToString());
        animationTree.Active = true;
    }

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
        var angleToNextPosition = forwardDirection.AngleTo(nextPositionDirection);

        if (angleToNextPosition > Mathf.DegToRad(MaximumTurnMoveAngle))
        {
            Velocity = Vector3.Zero;
            return;
        }

        Velocity = (nextPosition - GlobalPosition).Normalized() * MovementSpeed;

        MoveAndSlide();
    }

    protected override void HandleAnimations()
    {
        animationTree.Set(currentAnimations.GetDescription(), false);

        currentAnimations = !Velocity.IsZeroApprox() ? MrDuckAnimations.WalkForward : MrDuckAnimations.Idle;
        animationTree.Set(currentAnimations.GetDescription(), true);
    }

    protected override void Rotate(float delta)
    {
        interpolationCurrentPosition = interpolationCurrentPosition.Lerp(nextPosition, RotationSpeed);
        LookAt(interpolationCurrentPosition);
    }
}
