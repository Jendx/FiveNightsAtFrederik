using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.CsScripts.Models;
using Godot;
using System.Linq;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props.Camera_System;

public partial class CameraSystem : BaseInteractableNode3D/*, IIndirectlyUsable<CameraSystemParameters>*/
{
	[Export]
	private Camera3D[] _cameras;

	private SubViewport _viewport;
	private MeshInstance3D _cameraView;
	private int _cameraIndex;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_viewport = GetNode<SubViewport>(NodeNames.CameraSystemViewport.ToString());
		_cameraView = GetNode<MeshInstance3D>(NodeNames.CameraView.ToString());

		// Cameras must be parented to SubViewport or they would project to player
		foreach(var camera in _cameras)
		{
			camera.Reparent(_viewport);
		}

		if (!_cameras.Any())
		{
			GD.PrintErr($"{Name} Node has no cameras. Camera display won't work!");
		}
		
		RenderingServer.FramePostDraw += PostDrawHandler;
	}

	public void PostDrawHandler()
	{
		GD.Print("Albedo Set!");
		(_cameraView.MaterialOverride as BaseMaterial3D).AlbedoTexture = _viewport.GetTexture();
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
		foreach(var camera in _cameras)
		{
			camera.Current = false;
		}

		_cameraIndex = _cameraIndex == 0 ? 1 : 0;
		GD.Print($"Switched to camera: ${_cameras[_cameraIndex].Name}");
		_cameras[_cameraIndex].MakeCurrent();
	}
}
