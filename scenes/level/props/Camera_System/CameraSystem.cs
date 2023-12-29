using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props.Camera_System;

public partial class CameraSystem : Node3D, IUsable
{
	[Export]
	private Camera3D[] _cameras;

	private SubViewport _viewport;
	private MeshInstance3D _cameraView;
	private int _cameraIndex;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_viewport = GetNode<SubViewport>(StringNames.CameraSystemViewport.ToString());
		_cameraView = GetNode<MeshInstance3D>(StringNames.CameraView.ToString());

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

	public void OnBeginUse(bool isToggle)
	{
		SwitchCamera();
	}

	public void OnEndUse(bool isToggle) {}

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
