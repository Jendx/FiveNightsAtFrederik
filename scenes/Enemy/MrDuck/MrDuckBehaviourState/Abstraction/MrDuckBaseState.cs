using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourState.Enums;
using Godot;
using Godot.Collections;
using System;

namespace FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourFactory.Abstraction;

public abstract class MrDuckBaseState
{
    protected readonly Scenes.Enemy.MrDuck mrDuck;
    protected readonly AudioStreamPlayer3D audioPlayer;
    protected readonly Random random;
    protected readonly Dictionary<EnemySounds, AudioStreamMP3> audioTracks;
    protected readonly Timer idleTimer;

    protected MrDuckBaseState(
        Timer idleTimer,
        Dictionary<EnemySounds, AudioStreamMP3> audioTracks,
        Random random,
        AudioStreamPlayer3D audioPlayer,
        Scenes.Enemy.MrDuck mrDuck)
    {
        this.idleTimer = idleTimer;
        this.audioTracks = audioTracks;
        this.random = random;
        this.audioPlayer = audioPlayer;
        this.mrDuck = mrDuck;
    }

    public abstract void HandleBehaviour();
    public abstract MrDuckBehaviourStates HandleTargetReached();
}
