using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.CsScripts.Models.Interfaces;
using Godot;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props;

public partial class Button : Node, IButton, IPlayerUsable
{
	[Export]
	public bool IsToggle { get; set; } = false;

	[Export(hintString: "Time in ms")]
	public float DelayLength { get; set; }

	[Export]
	public Node? UsableNode { get; set; } 

	[Export]
	private BaseUsableParameters? parameters;

	[Export]
	public AudioStreamOggVorbis? SwitchOnAudio { get; set; }

	[Export]
	public AudioStreamOggVorbis? SwitchOffAudio { get; set; }

    [Export]
    public bool IsInteractionUIDisplayed { get; set; } = true;

    private bool isOnCoolDown;
	private AudioStreamPlayer? audioPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if (UsableNode is null or not IIndirectlyUsable<BaseUsableParameters>) 
		{
			GD.PrintErr($"{Name} UsableNode Is null and won't be executed");
		}

		audioPlayer = this.TryGetNode<AudioStreamPlayer>(NodeNames.UseAudio.ToString(), nameof(audioPlayer));
    }

	public async void OnBeginUse()
	{
		if (isOnCoolDown)
		{
			return;
		}

		isOnCoolDown = true;

		audioPlayer?.TrySetAndPlayStream(SwitchOnAudio);
        (UsableNode as IIndirectlyUsable<BaseUsableParameters>)?.OnBeginUse(parameters);

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
        (UsableNode as IIndirectlyUsable<BaseUsableParameters>)?.OnEndUse(parameters);
	}
}
