using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using Godot.Collections;
using System;

namespace FiveNightsAtFrederik.Scenes.Enemy;

/// <summary>
/// American Bug is enemy that after some time starts activating and rushes player
/// </summary>
public partial class AmericanBug : BaseEnemy, IMovableCharacter, IDamagable
{
    public float MovementSpeed { get; set; } = 400f;
    public float JumpVelocity { get; set; }
    public float RotationSpeed { get; set; } = 0.8f;

    private readonly Random random = new(2);
    private Timer activationTimer;

    [ExportGroup("Dictionary<EnemySounds, AudioStreamMp3> EnumValues: 0:Deactivate, 1:Activate, 2:Jumpscare")]
    [Export()]
    private Dictionary<EnemySounds, AudioStreamMP3> audioTracks = new();
    [ExportGroup("")]

    /// <summary>
    /// Slowly interpolates between ForwardPosition and nextPosition, which allows for smoot rotation 
    /// </summary>
    private Vector3 interpolationCurrentPosition;
    private Vector3 nextPosition;
    private Vector3 spawnLocation;
    private bool isHit;

    public override void _Ready()
    {
        base._Ready();
        activationTimer = GetNode<Timer>(NodeNames.BugActivationTimer.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(activationTimer)} at {NodeNames.BugActivationTimer}");
        activationTimer.Timeout += () => isActive = true;

        interpolationCurrentPosition = LookForwardPosition.GlobalPosition;
        spawnLocation = GlobalPosition;

        animationTree.Active = true;

        // TODO: Set correct activation timer value
        idleTimer.Start(5);
    }

    /// <summary>
    /// When target is reached, kill player.
    /// If bug was hit, Deactivate him after getting back to spawn location
    /// </summary>
    protected override void OnTargetReached()
    {
        isActive = false;

        if (isHit)
        {
            GD.Print("Bug Deactivated!");
            idleTimer.Start(random.Next(30, 60));
            isHit = false;

            return;
        }

        currentAnimation = EnemyAnimationStates.Jumpscare;
        animationTree.AnimationFinished += (_) => { GD.Print("U are ded"); };
        audioPlayer.Stream = audioTracks[EnemySounds.Jumpscare];
        audioPlayer.Play();
        
        player.HandleJumpscare(jumpscareCameraPositionMarker.GlobalPosition, GlobalPosition + new Vector3(0, 0.5f, 0));
    }

    /// <summary>
    /// Tries to activate bug If succeded, there is a warmup, before it is unleashed
    /// </summary>
    protected override void OnIdleTimerTimeout()
    {
        var number = random.Next(11);
        if (number > 3)
        {
            idleTimer.Start(random.Next(10, 20));

            GD.Print("Bug NOT Activated");
            return;
        }

        audioPlayer.Stream = audioTracks[EnemySounds.Activate];
        audioPlayer.Play();

        idleTimer.Stop();

        if (activationTimer.TimeLeft == 0)
        {
            GD.Print("Bug Activated");
            activationTimer.Start();
        }
    }

    protected override void Move(float delta)
    {
        if (!isHit)
        {
            navigationAgent.TargetPosition = player.GlobalPosition;
        }

        nextPosition = navigationAgent.GetNextPathPosition();

        Velocity = (nextPosition - GlobalPosition).Normalized() * delta * MovementSpeed;

        MoveAndSlide();
    }

    protected override void Rotate(float delta)
    {
        interpolationCurrentPosition = nextPosition.Lerp(interpolationCurrentPosition, delta * RotationSpeed);
        interpolationCurrentPosition.Y = GlobalPosition.Y;

        LookAt(interpolationCurrentPosition);
    }

    protected override void HandleAnimations()
    {
        animationTree.Set(currentAnimation.GetDescription(), false);

        if (currentAnimation != EnemyAnimationStates.Jumpscare)
        {
            currentAnimation = !Velocity.IsZeroApprox() ? EnemyAnimationStates.WalkForward : EnemyAnimationStates.Idle;
        }

        animationTree.Set(currentAnimation.GetDescription(), true);
    }


    /// <summary>
    /// When bug is shot, set targetPostion = spawnLocation
    /// </summary>
    public void ApplyDamage()
    {
        if (isActive)
        {
            navigationAgent.TargetPosition = spawnLocation;
            audioPlayer.Stop();
            isHit = true;
        }
    }
}
