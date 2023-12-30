using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.CsScripts.Models.Interfaces;
using Godot;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props.Button;

#nullable enable
public partial class Button : Node, IButton, IPlayerUsable
{
	[Export]
	public bool IsToggle { get; set; } = false;

	[Export(hintString: "Time in ms")]
	public float DelayLength { get; set; }

	[Export]
	public BaseInteractableNode3D? UsableNode { get; set; } 

	[Export]
	private BaseUsableParameters? _parameters;

	[Export]
	public AudioStreamOggVorbis? SwitchOnAudio { get; set; }

	[Export]
	public AudioStreamOggVorbis? SwitchOffAudio { get; set; }


	private bool isOnCoolDown;
	private AudioStreamPlayer? audioPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if (UsableNode is null) 
		{
			GD.PrintErr($"{Name} UsableNode Is null and won't be executed");
		}

		audioPlayer = GetNode<AudioStreamPlayer>(NodeNames.UseAudio.ToString());
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

		audioPlayer?.TrySetAndPlayStream(SwitchOnAudio);
        UsableNode?.OnBeginUse(_parameters);

		if (DelayLength != default)
		{
			var timer = GetTree().CreateTimer(DelayLength);
			await ToSignal(timer, "timeout");
		}

		isOnCoolDown = false;
	}

	public void OnEndUse()
	{
        audioPlayer?.TrySetAndPlayStream(SwitchOffAudio);
        UsableNode?.OnEndUse(_parameters);
	}
}
