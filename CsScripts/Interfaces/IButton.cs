using Godot;

namespace FiveNightsAtFrederik.CsScripts.Interfaces;

public interface IButton
{
    public bool IsToggle { get; set; }

    public float DelayLength { get; set; }

    public Node3D UsableNode { get; set; }

    public AudioStreamOggVorbis SwitchOnAudio { get; set; }
    public AudioStreamOggVorbis SwitchOffAudio { get; set; }
}
