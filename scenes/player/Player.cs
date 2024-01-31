using Godot;
using FiveNightsAtFrederik.CsScripts.Controllers;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.CsScripts.Constants;

namespace FiveNightsAtFrederik.Scenes.Player;

public partial class Player : CharacterBody3D, IMovableCharacter
{
	[Export]
	public float CrouchSpeed { get; set; } = 2f;
	
	[Export]
	public float WalkSpeed { get; set; } = 5f;	

	/// <summary>
	/// speed of player when transitioning to/from crouch 
	/// </summary>
	[Export]
	public float StandSpeed { get; set; } = 4f;

	[Export]
	public float JumpVelocity { get; set; } = 5f;

	[Export]
	public float RotationSpeed { get; set; } = 0.01f;

	[Export]
	public float StandHeight { get; set; } = 1f;

	[Export]
	public float CrouchHeight { get; set; } = 0.2f;

	public Camera3D Camera { get; set; }

	public CollisionShape3D CollisionMesh { get; set; }

	public Marker3D CarryableItemPositionMarker { get; set; }

	public float MovementSpeed => isCrouching ? CrouchSpeed : WalkSpeed;

	public bool isCrouching;

	[Signal]
	public delegate void UsableObjectChangedEventHandler(bool isUsableObject);

	[Signal]
	public delegate void OnRaycastColideEventHandler(Node colidedObject);

	private PlayerController PlayerController;

	private RayCast3D RayCast;

	public Player()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Ready()
	{
		Camera = GetNode<Camera3D>(NodeNames.Camera.ToString());
		RayCast = Camera.GetNode<RayCast3D>(NodeNames.Camera_RayCast.ToString());
		CarryableItemPositionMarker = Camera.GetNode<Marker3D>(NodeNames.Camera_CarryableItemPositionMarker.ToString());
		CollisionMesh = GetNode<CollisionShape3D>(NodeNames.PlayerCollision.ToString());
		PlayerController = new PlayerController(this);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed(ActionNames.DEBUG_TOGGLEMOUSE))
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
		}

		PlayerController.HandleCrouch(delta);
		PlayerController.HandleMovement(delta);
		PlayerController.UpdateLookAtObject(RayCast);

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
			PlayerController.RotateByMouseDelta(eventMouseMotion.Relative, Camera);
		}
	}
}
