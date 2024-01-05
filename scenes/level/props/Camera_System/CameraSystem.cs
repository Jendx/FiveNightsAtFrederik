using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using Godot;
using System.Linq;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props.Camera_System;

public partial class CameraSystem : RaycastInteractable2DUiNode3D
{
	[Export]
	private Camera3D[] cameras;

	[Export]
	private Control UI;
	private int cameraIndex;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		uiArea = GetNode<Area3D>("CameraView/CameraSystemViewArea");
		subViewport = GetNode<SubViewport>("CameraSystemViewport");
		cameraViewDisplayMesh = GetNode<MeshInstance3D>("CameraView");

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
}
