using Godot;
using FiveNightsAtFrederik.CsScripts.Controllers;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.CsScripts.Constants;
using System.Diagnostics;

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

	public Camera3D Camera { get; set; }
	public Marker3D CarryableItemPositionMarker { get; set; }

    private RayCast3D RayCast;

	[Signal]
	public delegate void UsableObjectChangedEventHandler(bool isUsableObject);

	[Signal]
	public delegate void OnRaycastColideEventHandler(Node colidedObject);

	private readonly PlayerController PlayerController;

	public Player()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		PlayerController = new PlayerController(this);
	}

	public override void _Ready()
	{
		Camera = GetNode<Camera3D>(NodeNames.Camera.ToString());
		RayCast = Camera.GetNode<RayCast3D>(NodeNames.Camera_RayCast.ToString());
		CarryableItemPositionMarker = Camera.GetNode<Marker3D>(NodeNames.Camera_CarryableItemPositionMarker.ToString());
    }

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed(ActionNames.DEBUG_TOGGLEMOUSE))
		{
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;

        }

		PlayerController.HandleMovement();
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
