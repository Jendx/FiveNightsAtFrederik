using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.CsScripts.Models.Interfaces;
using Godot;

namespace FiveNightsAtFrederik.CsScripts.Scenes.Level.Props.Button;

public partial class Button : Node, IButton, IPlayerUsable
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

	[Export]
	private CSharpScript _parameters;

	private bool isOnCoolDown;
	private AudioStreamPlayer audioPlayer;
	private IIndirectlyUsable<IUsableParameters>? _usableNodeScript;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if (UsableNode is null) 
		{
			GD.PrintErr($"{Name} UsableNode Is null and won't be executed");
		}

		audioPlayer = GetNode<AudioStreamPlayer>(NodeNames.UseAudio.ToString());
        _usableNodeScript = UsableNode.GetScript().Obj as IIndirectlyUsable<IUsableParameters>;

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

        _usableNodeScript?.OnBeginUse(null);

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

        _usableNodeScript.OnEndUse(null);
	}
}
