using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using System;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props.Button;

public partial class Button : Node, IButton
{
	[Export]
	public bool IsToggle { get; set; } = false;

	[Export(hintString: "Time in ms")]
	public float DelayLength { get; set; }

	[Export]
	public Node3D UsableNode { get; set; }

	[Export]
	public AudioStreamOggVorbis SwitchOnAudio { get; set; }

	[Export]
	public AudioStreamOggVorbis SwitchOffAudio { get; set; }

	private bool isOnCoolDown;
	private AudioStreamPlayer audioPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (UsableNode is null)
		{
			GD.PrintErr($"{Name} UsableNode Is null and won't be executed");
		}

		audioPlayer = GetNode<AudioStreamPlayer>("UseAudio");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public async void OnBeginUse()
	{
		if (isOnCoolDown)
		{
			return;
		}

		isOnCoolDown = true;
		audioPlayer.Stream = SwitchOnAudio;
		audioPlayer.Play();

		if (UsableNode is not null && UsableNode.HasMethod(nameof(IUsableNode.OnBeginUse)))
		{
			UsableNode.Call(nameof(IUsableNode.OnBeginUse), IsToggle);
		}

		if (DelayLength != default)
		{
			var timer = GetTree().CreateTimer(DelayLength);
			await ToSignal(timer, "timeout");
		}

		isOnCoolDown = false;
	}

	public void OnEndUse()
	{
		audioPlayer.Stream = SwitchOffAudio;
		audioPlayer.Play();

		if (UsableNode is not null && UsableNode.HasMethod(nameof(IUsableNode.OnEndUse)))
		{
			UsableNode.Call(nameof(IUsableNode.OnEndUse), IsToggle);
		}

	}
}
