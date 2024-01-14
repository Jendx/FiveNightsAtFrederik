using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Controllers;
using Godot;

namespace FiveNightsAtFrederik.Scenes.Enemy;

public partial class BaseEnemy : CharacterBody3D
{
	private NavigationAgent3D navigationAgent;
	private Player.Player player;
	private EnemyMasterController controller;
	public Marker3D CurrentMarker;

	[Export]
	private const float Speed = 2f;

	public override void _Ready()
	{
		controller = new EnemyMasterController(this);
		navigationAgent = GetNode<NavigationAgent3D>(NodeNames.NavigationAgent.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(navigationAgent)} at {NodeNames.NavigationAgent}");
		player = GetNode<Player.Player>(NodeNames.PlayerInRoot.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {NodeNames.PlayerInRoot}");

		CurrentMarker = controller.GetNextPossibleDestination();
		navigationAgent.TargetPosition = CurrentMarker.GlobalPosition;


		navigationAgent.TargetReached += () =>
		{
			CurrentMarker = controller.GetNextPossibleDestination();
			navigationAgent.TargetPosition = CurrentMarker.GlobalPosition;
			GD.Print($"Target Location switched to: {CurrentMarker.Name}");
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		var nextPosition = navigationAgent.GetNextPathPosition();
		Velocity = (nextPosition - GlobalPosition).Normalized() * Speed;

		MoveAndSlide();
	}
}
