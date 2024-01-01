using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using Godot;
using System.Linq;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props.Camera_System;

public partial class CameraSystem : RaycastInteractable2DUiNode3D
{
	[Export]
	private Camera3D[] cameras;

	private Area3D area;
	private int cameraIndex;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		cameraViewDisplayMesh = GetNode<MeshInstance3D>(NodeNames.CameraView.ToString());

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
