using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.scenes.Enemy.MrDuck;
using Godot;
using Godot.Collections;
using System;

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
    private Random random = new();

    [ExportGroup("Dictionary<MrDuckSounds, AudioStreamMp3> EnumValues: 0:Deactivate, 1:Activate, 2:Jumpscare")]
    [Export()]
    private Dictionary<MrDuckSounds, AudioStreamMP3> audioTracks = new();
    [ExportGroup("")]

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

        idleTimer.Start(5);
    }

    protected override void OnTargetReached()
    {
        CurrentMarker = controller.GetNextPossibleDestination();
        navigationAgent.TargetPosition = CurrentMarker.GlobalPosition;
        GD.Print($"Target Location switched to: {CurrentMarker.Name}");
    }

    /// <summary>
    /// Determines when is MrDuck active. Basicly this is very simple state machine
    /// </summary>
    protected override void OnIdleTimerTimeout()
    {
        var number = random.Next(11);
        if (number > 3)
        {
            idleTimer.Start(random.Next(10, 20));
            audioPlayer.Stream = audioTracks[MrDuckSounds.Deactivate];
            if (!isActive)
            {
                audioPlayer.Play();
            }

            isActive = false;
            GD.Print("Duck Deactivated");
            return;
        }


        GD.Print("Duck Activated");
        isActive = true;
        audioPlayer.Stream = audioTracks[MrDuckSounds.Activate];
        if (isActive)
        {
            audioPlayer.Play();
        }
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
