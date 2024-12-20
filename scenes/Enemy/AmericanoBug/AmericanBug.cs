using FiveNightsAtFrederik.CsScripts.BaseNodes;
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
	public float RotationSpeed { get; set; } = 7f;

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
		activationTimer.Timeout += () => IsActive = true;

		interpolationCurrentPosition = LookForwardMarker.GlobalPosition;
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
		IsActive = false;

		if (isHit)
		{
			GD.Print("Bug Deactivated!");
			idleTimer.Start(random.Next(30, 60));
			isHit = false;

			return;
		}

		CurrentAnimation = EnemyAnimationStates.Jumpscare;
		audioPlayer.Stream = audioTracks[EnemySounds.Jumpscare];
		audioPlayer.Play();
		
		player.HandleJumpscare(JumpscareCameraPositionMarker.GlobalPosition, GlobalPosition + new Vector3(0, 0.5f, 0));
	}

	/// <summary>
	/// Tries to activate bug If succeeded, there is a warmup, before it is unleashed
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
			NavigationAgent.TargetPosition = player.GlobalPosition;
		}

		nextPosition = NavigationAgent.GetNextPathPosition();
		Velocity = (nextPosition - GlobalPosition).Normalized() * delta * MovementSpeed;
		MoveAndSlide();
	}

	protected override void Rotate(float delta)
	{
		this.RotateYByShortestWayToTarget(LookForwardMarker, nextPosition, RotationSpeed, delta);
	}


	/// <summary>
	/// When bug is shot, set targetPostion = spawnLocation
	/// </summary>
	public void ApplyDamage()
	{
		if (IsActive)
		{
			NavigationAgent.TargetPosition = spawnLocation;
			audioPlayer.Stop();
			isHit = true;
		}
	}
}
