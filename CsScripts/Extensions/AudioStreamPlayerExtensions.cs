using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveNightsAtFrederik.CsScripts.Extensions;

public static class AudioStreamPlayerExtensions
{
    /// <summary>
    /// Sets Stream of player to audioStream and plays it
    /// </summary>
    /// <param name="audioStream">Stream to be played</param>
    public static void TrySetAndPlayStream(this AudioStreamPlayer audioPlayer, AudioStream audioStream)
    {
        audioPlayer.Stream = audioStream;
        audioPlayer.Play();
    }
}
