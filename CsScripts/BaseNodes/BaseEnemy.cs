using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Controllers;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using Godot;
using System;
using System.Linq;

namespace FiveNightsAtFrederik.Scenes.Enemy;

public partial class BaseEnemy : CharacterBody3D
{
    public bool IsActive { get; set; }
    public Marker3D CurrentDestinationMarker { get; set; }
    public NavigationAgent3D NavigationAgent { get; protected set; }
    public Marker3D JumpscareCameraPositionMarker { get; protected set; }

	protected bool isFirstDestinationSet;
    protected Player.Player player;
	protected Marker3D LookForwardMarker;
    protected Timer idleTimer;
    protected AudioStreamPlayer3D audioPlayer;
    protected AnimationTree animationTree;
    protected EnemyAnimationStates currentAnimation = EnemyAnimationStates.Idle;

    /// <summary>
    /// Move method responsible for moving the enemy
    /// </summary>
    protected virtual void Move(float delta) => throw new NotImplementedException();

    /// <summary>
    /// Rotate method responsible for rotating the enemy
    /// </summary>
    protected virtual void Rotate(float delta) => throw new NotImplementedException();

    /// <summary>
    /// Method for handling animations
    /// </summary>
    protected virtual void HandleAnimations() => throw new NotImplementedException();

    /// <summary>
    /// Defines what happens when enemy reaches point
    /// </summary>
    protected virtual void OnTargetReached() => throw new NotImplementedException();

    /// <summary>
    /// Defines what happens when idleTimer finishes
    /// </summary>
    protected virtual void OnIdleTimerTimeout() => throw new NotImplementedException();

    public override void _Ready()
    {
        player = GetTree().GetNodesInGroup(GroupNames.playerGroup.ToString()).FirstOrDefault() as Player.Player ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {NodeNames.PlayerInRoot}");
        NavigationAgent = this.TryGetNode<NavigationAgent3D>(NodeNames.NavigationAgent, nameof(NavigationAgent));
        LookForwardMarker = this.TryGetNode<Marker3D>(NodeNames.LookForwardPosition, nameof(LookForwardMarker));
        idleTimer = this.TryGetNode<Timer>(NodeNames.IdleTimer, nameof(idleTimer));
        audioPlayer = this.TryGetNode<AudioStreamPlayer3D>(NodeNames.AudioPlayer, nameof(audioPlayer));
        animationTree = this.TryGetNode<AnimationTree>(NodeNames.AnimationTree, nameof(animationTree));
        JumpscareCameraPositionMarker = this.TryGetNode<Marker3D>(NodeNames.JumpscareCameraPosition, nameof(JumpscareCameraPositionMarker));

        animationTree.AnimationFinished += (animationName) =>
        {
            if (string.Equals(animationName.ToString() == EnemyAnimationStates.Jumpscare.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                //TODO: Add Proper death screen
                GD.Print("U are ded");
            }
        };

        NavigationAgent.TargetReached += OnTargetReached;
        idleTimer.Timeout += OnIdleTimerTimeout;
    }

    /// <summary>
    /// Processes Enemy actions if enemy is Active
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta) 
	{
        HandleAnimations();

        if (!IsActive) 
        {
            return;
        }

		Rotate((float)delta);
		Move((float)delta);
	}
}
