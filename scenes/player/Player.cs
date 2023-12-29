using FiveNightsAtFrederik.CsScripts.Handlers;
using FiveNightsAtFrederik.CsScripts.Controllers.Interfaces;
using Godot;
using FiveNightsAtFrederik.CsScripts;

namespace FiveNightsAtFrederik.Scenes.Player;

public partial class Player : CharacterBody3D, IMovableCharacter
{
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	[Export]
	public float MovementSpeed { get; set; } = 5f;

	[Export]
	public float JumpVelocity { get; set; } = 5f;

	[Export]
	public float RotationSpeed { get; set; } = 0.01f;

	public readonly PlayerController PlayerController;
	private Camera3D _camera;
	private RayCast3D _rayCast;

	public Player()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;

		PlayerController = new PlayerController(this);
	}

	public override void _Ready()
	{
		_camera = this.GetNode<Camera3D>(StringNames.Camera.ToString());
		_rayCast = _camera.GetNode<RayCast3D>(StringNames.RayCast.ToString());

		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		PlayerController.HandleMovement();

		if (Input.IsActionJustPressed("Use"))
		{
			PlayerController.UseRayCast(_rayCast);
		}

		if (Input.IsActionJustReleased("Use"))
		{
			PlayerController.StopUsingRayCast();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			PlayerController.RotateByMouseDelta(eventMouseMotion.Relative, _camera);
		}
	}
}
