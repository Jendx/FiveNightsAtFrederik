using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using Godot;
using System.Linq;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props.Camera_System;

public partial class CameraSystem : InteractableNode3D
{
	[Export]
	private Camera3D[] cameras;

	private Area3D area;
	private SubViewport subViewport;
	private MeshInstance3D cameraView;
	private int cameraIndex;

	private bool isMouseHeld;
    private bool isMouseInside;
    private Vector3? lastMousePosition3D;
    private Vector2 quadSize;
    private Vector2 lastMousePosition2D = Vector2.Zero;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		subViewport = GetNode<SubViewport>(NodeNames.CameraSystemViewport.ToString());
		cameraView = GetNode<MeshInstance3D>(NodeNames.CameraView.ToString());
        area = GetNode<Area3D>(NodeNames.CameraSystemViewArea.ToString());

		// Cameras must be parented to SubViewport or they would project to player
		foreach(var camera in cameras)
		{
			camera.Reparent(subViewport);
		}

		if (!cameras.Any())
		{
			GD.PrintErr($"{Name} Node has no cameras. Camera display won't work!");
		}
		
		RenderingServer.FramePostDraw += PostDrawHandler;
    }

	/// <summary>
	/// Textures must be set here
	/// </summary>
    public void PostDrawHandler()
	{
		GD.Print("Albedo Set!");
		(cameraView.MaterialOverride as BaseMaterial3D).AlbedoTexture = subViewport.GetTexture();
		RenderingServer.FramePostDraw -= PostDrawHandler;
	}

	public override void OnBeginUse<CameraSystemParameters>(CameraSystemParameters parameters)
	{
		SwitchCamera();
	}

	public override void OnEndUse<CameraSystemParameters>(CameraSystemParameters parameters) {}
    
    public override void _UnhandledInput(InputEvent @event)
    {
        bool isMouseEvent = @event is InputEventMouse or InputEventMouseMotion or InputEventMouseButton;

        if (isMouseEvent && (isMouseInside || isMouseHeld))
        {
            GD.Print("Mouse Handled");
            HandleMouse((InputEventMouse)@event);

        }

        if (!isMouseEvent)
        {
            GD.Print("Inpunt Pushed to SV");
            subViewport.PushInput(@event);
            return;
        }

    }

	private void SwitchCamera()
	{
		// Just for testing. Will be implemented more complexly with UI
		foreach(var camera in cameras)
		{
			camera.Current = false;
		}

		cameraIndex = cameraIndex == 0 ? 1 : 0;
		GD.Print($"Switched to camera: ${cameras[cameraIndex].Name}");
		cameras[cameraIndex].MakeCurrent();
	}

    public void Player_onRayCastColided(Node colidedObject)
	{
		GD.Print("Mouse Entered");
		isMouseInside = true;
    }

    private void HandleMouse(InputEventMouse @event)
    {
        isMouseInside = FindMouse(@event.GlobalPosition, out Vector3 position);

        HandleMouseInPosition(@event, position);
    }

    public void HandleSynteticMouseMotion(Vector3 position)
    {
        var ev = new InputEventMouseMotion();

        isMouseInside = true;

        HandleMouseInPosition(ev, position);
    }

    public void HandleSynteticMouseClick(Vector3 position, bool pressed)
    {
        var ev = new InputEventMouseButton() { ButtonIndex = MouseButton.Left, Pressed = pressed };

        isMouseInside = true;

        HandleMouseInPosition(ev, position);
    }

    private void HandleMouseInPosition(InputEventMouse @event, Vector3 position)
    {
        quadSize = (cameraView.Mesh as QuadMesh).Size;

        if (@event is InputEventMouseButton)
        {
            isMouseHeld = @event.IsPressed();
        }

        Vector3 mousePosition3D;

        if (isMouseInside)
        {
            mousePosition3D = area.GlobalTransform.AffineInverse() * position;
            lastMousePosition3D = mousePosition3D;
        }
        else
        {
            mousePosition3D = lastMousePosition3D is null ? Vector3.Zero : (Vector3)lastMousePosition3D;
        }

        var mousePosition2D = new Vector2(mousePosition3D.X, -mousePosition3D.Y);

        mousePosition2D.X += quadSize.X / 2;
        mousePosition2D.Y += quadSize.Y / 2;

        mousePosition2D.X /= quadSize.X;
        mousePosition2D.Y /= quadSize.Y;

        mousePosition2D.X *= subViewport.Size.X;
        mousePosition2D.Y *= subViewport.Size.Y;

        @event.Position = mousePosition2D;
        @event.GlobalPosition = mousePosition2D;

        if (@event is InputEventMouseMotion)
        {
            (@event as InputEventMouseMotion).Relative = mousePosition2D - lastMousePosition2D;
        }

        lastMousePosition2D = mousePosition2D;

        subViewport.PushInput(@event);
    }

    private bool FindMouse(Vector2 globalPosition, out Vector3 position)
    {
        var camera = GetViewport().GetCamera3D();

        var from = camera.ProjectRayOrigin(globalPosition);
        var dist = FindFurtherDistanceTo(camera.Transform.Origin);
        var to = from + camera.ProjectRayNormal(globalPosition) * dist;

        var parameters = new PhysicsRayQueryParameters3D() { From = from, To = to, CollideWithAreas = true, CollisionMask = area.CollisionLayer, CollideWithBodies = false };

        var result = GetWorld3D().DirectSpaceState.IntersectRay(parameters);

        position = Vector3.Zero;

        if (result.Count > 0)
        {
            position = (Vector3)result["position"];

            return true;
        }
        else
        {
            return false;
        }
    }

    private float FindFurtherDistanceTo(Vector3 origin)
    {
        Vector3[] edges = new Vector3[] {
            area.ToGlobal(new Vector3(quadSize.X / 2, quadSize.Y / 2, 0)),
            area.ToGlobal(new Vector3(quadSize.X / 2, -quadSize.Y / 2, 0)),
            area.ToGlobal(new Vector3(-quadSize.X / 2, quadSize.Y / 2, 0)),
            area.ToGlobal(new Vector3(-quadSize.X / 2, -quadSize.Y / 2, 0)),
        };

        float farDistance = 0;

        foreach (var edge in edges)
        {
            var tempDistance = origin.DistanceTo(edge);

            if (tempDistance > farDistance)
            {
                farDistance = tempDistance;
            }
        }

        return farDistance;
    }
}
