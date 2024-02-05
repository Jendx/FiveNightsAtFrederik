using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Controllers;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourFactory;
using FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourFactory.Abstraction;
using FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourState.Enums;
using Godot;
using Godot.Collections;
using System;

namespace FiveNightsAtFrederik.Scenes.Enemy;

/// <summary>
/// Mr Duck is enemy which should move like
/// </summary>
public partial class MrDuck : BaseEnemy, IMovableCharacter
{
    public float MovementSpeed { get; private set; } = 32f;
    public float JumpVelocity { get; set; }
    public float RotationSpeed { get; set; } = 1f;
    public Sight sight { get; private set; }
    public Timer ChaseCooldownTimer { get; private set; }

    private readonly float MaximumTurnMoveAngle = 5;
    private readonly Random random = new(1);

    private MrDuckBehaviourStateManager behaviourController;
    private MrDuckBaseState currentBehaviourState;
    private MrDuckBehaviourStates currentBehaviour = MrDuckBehaviourStates.Roam;

    [ExportGroup("Dictionary<EnemySounds, AudioStreamMp3> EnumValues: 0:Deactivate, 1:Activate, 2:Jumpscare")]
    [Export()]
    private Dictionary<EnemySounds, AudioStreamMP3> audioTracks = new();
    [ExportGroup("")]

    /// <summary>
    /// Slowly interpolates between ForwardPosition and nextPosition, which allows for smoot rotation 
    /// </summary>
    private Vector3 interpolationCurrentPosition;
    private Vector3 nextPosition;

    public override void _Ready()
    {
        base._Ready();
        sight = this.TryGetNode<Sight>(NodeNames.Sight, nameof(sight));
        sight.OnPlayerSighted += () =>
        {
            if (IsActive)
            {
                currentBehaviour = MrDuckBehaviourStates.Chase;
                currentBehaviourState = behaviourController.UpdateBehaviour(currentBehaviour);
                currentBehaviourState.HandleBehaviour();
            }
        };

        ChaseCooldownTimer = this.TryGetNode<Timer>(NodeNames.ChaseCooldownTimer, nameof(ChaseCooldownTimer));
        interpolationCurrentPosition = LookForwardMarker.GlobalPosition;
        animationTree.Active = true;

        // TODO: Set correct activation Time
        idleTimer.Start(5);

        behaviourController = new MrDuckBehaviourStateManager(this, audioPlayer, random, audioTracks, idleTimer, player);
        currentBehaviourState = behaviourController.UpdateBehaviour(currentBehaviour);
    }

    /// <summary>
    /// In here it is used as init only... & calculateSpeed
    /// When ready is fired, the navigation is not ready yet
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta)
    {
        CalculateMovementSpeed();
        if (isFirstDestinationSet)
        {
            return;
        }

        // Get first destination
        currentBehaviourState.HandleTargetReached();
        isFirstDestinationSet = true;
    }

    /// <summary>
    /// Get next position to roam to or Execute jumpscare
    /// </summary>
    protected override void OnTargetReached()
    {
        currentBehaviour = currentBehaviourState.HandleTargetReached();
    }

    /// <summary>
    /// Determines when is MrDuck active. Basically this is very simple state machine
    /// </summary>
    protected override void OnIdleTimerTimeout()
    {
        currentBehaviourState = behaviourController.UpdateBehaviour(currentBehaviour);
        currentBehaviourState.HandleBehaviour();
    }

    protected override void Move(float delta)
    {
        nextPosition = NavigationAgent.GetNextPathPosition();

        // Calculates vector with direction from GlobalPosition to nextPosition
        var nextPositionDirection = nextPosition.DirectionTo(GlobalPosition);

        // Calculates vector with direction from LookForwardPosition to GlobalPosition So I do not have to figure out how to get forward vector in Global Cords 
        var forwardDirection = LookForwardMarker.GlobalPosition.DirectionTo(GlobalPosition);
        var angleToNextPosition = forwardDirection.AngleTo(nextPositionDirection);

        if (angleToNextPosition > Mathf.DegToRad(MaximumTurnMoveAngle))
        {
            Velocity = Vector3.Zero;
            return;
        }

        Velocity = (nextPosition - GlobalPosition).Normalized() * delta * MovementSpeed;

        MoveAndSlide();
    }

    protected override void Rotate(float delta)
    {
        this.RotateYByShortestWayToTarget(LookForwardMarker, nextPosition, RotationSpeed, delta);
    }

    /// <summary>
    /// Calculates movement speed for next frame
    /// Handles interpolating between current MovementSpeed and CurrentStateSpeed
    /// </summary>
    /// <returns></returns>
    private float CalculateMovementSpeed()
    {
        MovementSpeed = Mathf.Lerp(MovementSpeed, (float)currentBehaviour, 0.5f);
        return MovementSpeed;
    }
}
