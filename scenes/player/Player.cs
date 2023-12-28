using FiveNightsAtFrederik.CsScripts.Handlers;
using FiveNightsAtFrederik.CsScripts.Controllers.Interfaces;
using Godot;

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

	public readonly PlayerMovementHandler PlayerMovementHandler;
	private Camera3D _camera;

	public Player() 
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;

		PlayerMovementHandler = new PlayerMovementHandler(this);
	}

	public override void _Ready()
	{
		_camera = this.GetNode<Camera3D>("Camera");
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;


		PlayerMovementHandler.HandleMovement();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			PlayerMovementHandler.RotateByMouseDelta(eventMouseMotion.Relative, _camera);
		}
	}
}
