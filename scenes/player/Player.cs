using Godot;
using FiveNightsAtFrederik.CsScripts.Controllers;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.CsScripts.Constants;

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

	[Signal]
	public delegate void UsableObjectChangedEventHandler(bool isUsableObject);

	public Player()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;

		PlayerController = new PlayerController(this);
	}

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>(NodeNames.Camera.ToString());
		_rayCast = _camera.GetNode<RayCast3D>(NodeNames.RayCast.ToString());
	}

	public override void _PhysicsProcess(double delta)
	{
		PlayerController.HandleMovement();

		if (Input.IsActionJustPressed(ActionNames.Use))
		{
			PlayerController.Use();
		}

		if (Input.IsActionJustReleased(ActionNames.Use))
		{
			PlayerController.StopUsing();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			PlayerController.RotateByMouseDelta(eventMouseMotion.Relative, _camera);
			PlayerController.UpdateLookAtObject(_rayCast);
		}
	}
}
