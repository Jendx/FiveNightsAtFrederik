using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using Godot;
using System;
using System.Linq;

namespace FiveNightsAtFrederik.Scenes.Enemy;

public partial class Sight : Node3D
{
	public bool IsPlayerInSight { get; set; }

	private Area3D sightArea;
	private RayCast3D rayCast;
	private Player.Player player;
	private Timer recastTimer;

	public delegate void PlayerSightedEventHandler();
	public event PlayerSightedEventHandler OnPlayerSighted;

	public override void _Ready()
	{
		player = GetTree().GetNodesInGroup(GroupNames.PlayerGroup.ToString()).FirstOrDefault() as Player.Player ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {NodeNames.PlayerInRoot}");
		rayCast = this.TryGetNode<RayCast3D>(NodeNames.RayCast, nameof(rayCast));
		sightArea = this.TryGetNode<Area3D>(NodeNames.VisionArea, nameof(sightArea));
		recastTimer = this.TryGetNode<Timer>(NodeNames.RecastTimer, nameof(recastTimer));

		sightArea.BodyEntered += SightArea_BodyEntered;
		sightArea.BodyExited += SightArea_BodyExited;
		recastTimer.Timeout += RecastTimer_Timeout;
	}

	private void SightArea_BodyEntered(Node3D body)
	{
		IsPlayerInSight = body is Player.Player;
		
		if (IsPlayerInSight)
		{
			recastTimer.Start();
		}
	}

	private void SightArea_BodyExited(Node3D body)
	{
		IsPlayerInSight = body is not Player.Player;

		if (!IsPlayerInSight)
		{
			recastTimer.Stop();
		}
	}

	private void RecastTimer_Timeout()
	{
		if (!IsPlayerInSight)
		{
			return;
		}

		rayCast.LookAt(player.CollisionMesh.GlobalPosition);
		var collidedObject = rayCast.GetCollider();
		var playerCollision = collidedObject.TryConvertTo<Player.Player>();

		if (playerCollision is null)
		{
			return;
		}

		OnPlayerSighted?.Invoke();
	}
}
