using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Controllers;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using System;

namespace FiveNightsAtFrederik.Scenes.Enemy;

[GlobalClass]
public partial class BaseEnemy : CharacterBody3D
{
    protected NavigationAgent3D navigationAgent;
    protected Player.Player player;
    protected EnemyMasterController controller;
	protected bool isFirstDestinationSet;
	protected Marker3D LookForwardPosition;

	public Marker3D CurrentMarker { get; set; }

    /// <summary>
    /// Move method responsible for moving the enemy
    /// </summary>
    protected virtual void Move(float delta) => throw new NotImplementedException();

    /// <summary>
    /// Rotate method responsible for rotating the enemy
    /// </summary>
    protected virtual void Rotate(float delta) => throw new NotImplementedException();

    /// <summary>
    /// Defines what happens when enemy reaches point
    /// </summary>
    protected virtual void OnTargetReached() => throw new NotImplementedException();

    public override void _Ready()
	{
		controller = new EnemyMasterController(this);
		navigationAgent = GetNode<NavigationAgent3D>(NodeNames.NavigationAgent.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(navigationAgent)} at {NodeNames.NavigationAgent}");
		player = GetNode<Player.Player>(NodeNames.PlayerInRoot.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {NodeNames.PlayerInRoot}");
        LookForwardPosition = GetNode<Marker3D>(NodeNames.LookForwardPosition.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {NodeNames.LookForwardPosition}");

		navigationAgent.TargetReached += OnTargetReached;
	}

	public override void _PhysicsProcess(double delta) 
	{
		Rotate((float)delta);
		Move((float)delta);
	}

    public override void _Process(double delta)
    {
		if (isFirstDestinationSet)
		{
			return;
		}

        CurrentMarker = controller.GetNextPossibleDestination();
        navigationAgent.TargetPosition = CurrentMarker.GlobalPosition;

		isFirstDestinationSet = true;
    }
}
