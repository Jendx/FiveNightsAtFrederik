using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourFactory.Abstraction;
using FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourState.Enums;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;
using Godot.Collections;
using System;

namespace FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourFactory;

#nullable enable
/// <summary>
/// Handles complete behaviour of MrDuck
/// </summary>
public class MrDuckBehaviourStateManager
{
    protected readonly MrDuckRoamState roamState;
    protected readonly MrDuckChaseState chaseState;

    public MrDuckBehaviourStateManager(
        Scenes.Enemy.MrDuck mrDuck,
        AudioStreamPlayer3D audioPlayer,
        Random random,
        Dictionary<EnemySounds, AudioStreamMP3> audioTracks,
        Timer idleTimer,
        Player player)
    {
        roamState = new MrDuckRoamState(mrDuck, audioPlayer, random, audioTracks, idleTimer);
        chaseState = new MrDuckChaseState(mrDuck, audioPlayer, random, audioTracks, idleTimer, player);
    }

    public MrDuckBaseState? UpdateBehaviour(MrDuckBehaviourStates currentBehaviour)
    {
        return currentBehaviour switch
        {
            MrDuckBehaviourStates.Roam => roamState,
            MrDuckBehaviourStates.Chase => chaseState,
            _ => null,
        };
    }
}
