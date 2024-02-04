using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourFactory.Abstraction;
using FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourState.Enums;
using FiveNightsAtFrederik.Scenes.Enemy;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;
using Godot.Collections;
using System;

namespace FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourFactory;

public class MrDuckChaseState : MrDuckBaseState
{
    private readonly Player player;

    public MrDuckChaseState(
        Scenes.Enemy.MrDuck mrDuck,
        AudioStreamPlayer3D audioPlayer,
        Random random,
        Dictionary<EnemySounds, AudioStreamMP3> audioTracks,
        Timer idleTimer,
        Player player) : base(idleTimer, audioTracks, random, audioPlayer, mrDuck)
    {
        this.player = player;
    }

    public override void HandleBehaviour()
    {
        mrDuck.IsActive = true;

        // If player is in sight the duck will follow the player
        // If not the duck will go to last know player position
        if (mrDuck.sight.IsPlayerInSight)
        {
            mrDuck.NavigationAgent.TargetPosition = player.GlobalPosition;
            return;
        }
    }

    /// <summary>
    /// If duck is near player, kill player
    /// </summary>
    public override MrDuckBehaviourStates HandleTargetReached()
    {
        if (mrDuck.GlobalPosition.DistanceTo(player.GlobalPosition) <= mrDuck.NavigationAgent.TargetDesiredDistance)
        {
            player.HandleJumpscare(mrDuck.JumpscareCameraPositionMarker.GlobalPosition, mrDuck.GlobalPosition);
        }

        //TODO: Set to Roam when not found
        return MrDuckBehaviourStates.Chase;
    }
}