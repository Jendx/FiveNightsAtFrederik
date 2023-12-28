using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using System;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Levels.Props.Props_lvl_1.Button;

public partial class Button : Node, IButton
{
	[Export]
	public bool IsToggle { get; set; } = false;

	[Export(hintString: "Time in ms")]
	public float DelayLength { get; set; }

	[Export]
	public Node3D UsableNode { get; set; }

	private bool isOnCoolDown;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (UsableNode is null)
		{
			GD.PrintErr($"{this.Name} UsableNode Is null and won't be executed");
		}
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
		if (UsableNode.HasMethod(nameof(IUsableNode.OnBeginUse)))
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

	public async void OnEndUse()
	{
		if (isOnCoolDown)
		{
			return;
		}

		isOnCoolDown = true;
		if (UsableNode.HasMethod(nameof(IUsableNode.OnEndUse)))
		{
			UsableNode.Call(nameof(IUsableNode.OnEndUse), IsToggle);
		}

		if (DelayLength != default)
		{
			var timer = GetTree().CreateTimer(DelayLength);
			await ToSignal(timer, "timeout");
		}

		isOnCoolDown = false;
	}
}
