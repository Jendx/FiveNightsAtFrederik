using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Models.Interfaces;
using Godot;

namespace FiveNightsAtFrederik.CsScripts.Interfaces;

public interface IButton : IPlayerUsable
{
    public bool IsToggle { get; set; }

    public float DelayLength { get; set; }

    public Node? UsableNode { get; set; }

    public AudioStreamOggVorbis SwitchOnAudio { get; set; }
    public AudioStreamOggVorbis SwitchOffAudio { get; set; }
}
