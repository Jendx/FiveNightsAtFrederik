using FiveNightsAtFrederik.CsScripts;
using Godot;

public partial class CameraSystem : Node3D
{
	[Export]
	private Camera3D[] _cameras;
	
	private SubViewport _viewport;
	private MeshInstance3D _cameraView;

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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
