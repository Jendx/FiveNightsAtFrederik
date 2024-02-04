using FiveNightsAtFrederik.CsScripts.Constants;
using Godot;
using static Godot.GodotObject;

namespace FiveNightsAtFrederik.CsScripts.Extensions;

public static class AudioPlayerExtensions
{
    /// <summary>
    /// Plays stream from position
    /// </summary>
    /// <param name="player"></param>
    /// <param name="audioStream"></param>
    public static void PlayStream(this AudioStreamPlayer3D player, AudioStream audioStream, float fromPosition = 0)
    {
        player.Stream = audioStream;
        player.Play(fromPosition);
    }
}
