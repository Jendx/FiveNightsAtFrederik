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

	public override void OnBeginUse<CameraSystemParameters>(CameraSystemParameters parameters)
	{
		SwitchCamera();
	}

	public override void OnEndUse<CameraSystemParameters>(CameraSystemParameters parameters) {}

    private void SwitchCamera()
    {
        // Just for testing. Will be implemented more complexly with UI
        foreach (var camera in cameras)
        {
            camera.Current = false;
        }

        cameraIndex = cameraIndex == 0 ? 1 : 0;
        GD.Print($"Switched to camera: ${cameras[cameraIndex].Name}");
        cameras[cameraIndex].MakeCurrent();
    }

}
