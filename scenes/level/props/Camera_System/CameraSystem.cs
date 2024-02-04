using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using System.Linq;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props;

/// <summary>
/// Camera system which loads it's child cameras & one Control
/// </summary>
public partial class CameraSystem : RaycastInteractable2DUiNode3D, IPlayerUsable
{
    [Export]
    public bool IsInteractionUIDisplayed { get; set; } = true;

    private Camera3D[] cameras;
	private Control UI;
	private int cameraIndex;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		uiArea = this.TryGetNode<Area3D>(NodeNames.CameraSystemViewArea, nameof(uiArea)); 
        subViewport = this.TryGetNode<SubViewport>(NodeNames.CameraSystemViewport, nameof(subViewport));
        cameraViewDisplayMesh = this.TryGetNode<MeshInstance3D>(NodeNames.CameraView, nameof(cameraViewDisplayMesh)); 
        var children = GetChildren();

		cameras = children.Where(ch => ch is Camera3D).Cast<Camera3D>().ToArray();
		UI = (Control)children.SingleOrDefault(ch => ch is Control);

		// Cameras must be parented to SubViewport or they would project to player
		foreach (var camera in cameras)
		{
			camera.Reparent(subViewport);
		}

		UI?.Reparent(subViewport);

		if (!cameras.Any() && UI is null)
		{
			GD.PrintErr($"{Name} Node has no cameras or UI. Display won't work! Please add cmaeras to Array and/or Control");
		}
		
		RenderingServer.FramePostDraw += PostDrawHandler;
	}

	/// <summary>
	/// Textures must be set here
	/// </summary>
	public void PostDrawHandler()
	{
		(cameraViewDisplayMesh.MaterialOverride as BaseMaterial3D).AlbedoTexture = subViewport.GetTexture();
		RenderingServer.FramePostDraw -= PostDrawHandler;
	}

	public void SwitchToCamera(string cameraName)
	{
		var selectedCamera = cameras?.FirstOrDefault(c => c.Name.ToString().Contains(cameraName));
		if (selectedCamera is null)
		{
			GD.PrintErr($"Camera with name {cameraName} was not found");
			return;
		}

		foreach (var camera in cameras)
		{
			camera.Current = false;
		}

		GD.Print($"Switched to camera: ${selectedCamera?.Name}");
		selectedCamera?.MakeCurrent();
	}

    public void OnBeginUse()
    {
    }

    public void OnEndUse()
    {
    }
}
