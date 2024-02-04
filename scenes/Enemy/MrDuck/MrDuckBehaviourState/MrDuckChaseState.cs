using FiveNightsAtFrederik.CsScripts.Controllers;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Helpers;
using FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourFactory.Abstraction;
using FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourState.Enums;
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
        mrDuck.ChaseCooldownTimer.Timeout += ChaseCooldownTimer_OnTimeout;
    }

    public override void HandleBehaviour()
    {
        mrDuck.NavigationAgent.TargetDesiredDistance = 2.5f;
        GD.Print(mrDuck.NavigationAgent.DistanceToTarget());

        // If player is in sight the duck will follow the player
        // If not the duck will go to last know player position
        if (mrDuck.sight.IsPlayerInSight)
        {
            audioPlayer.PitchScale = MathF.Max(0.1f, MathFHelper.Map(mrDuck.NavigationAgent.DistanceToTarget(), .0f, 7, 1.5f, 3.2f));
            audioPlayer.PlayStream(audioTracks[EnemySounds.Chase]);
            mrDuck.NavigationAgent.TargetPosition = player.GlobalPosition;
            mrDuck.ChaseCooldownTimer.Stop();

            return;
        }

    }

    /// <summary>
    /// If duck is near player, kill player
    /// </summary>
    public override MrDuckBehaviourStates HandleTargetReached()
    {
        audioPlayer.PitchScale = 1;
        if (mrDuck.GlobalPosition.DistanceTo(player.GlobalPosition) <= mrDuck.NavigationAgent.TargetDesiredDistance)
        {
            mrDuck.CurrentAnimation = EnemyAnimationStates.Jumpscare;
            audioPlayer.PlayStream(audioTracks[EnemySounds.Jumpscare]);
            mrDuck.IsActive = false;

            player.HandleJumpscare(mrDuck.JumpscareCameraPositionMarker.GlobalPosition, mrDuck.GlobalPosition + new Vector3(0, 0.5f, 0));

            return MrDuckBehaviourStates.Roam;
        }

        EnemyMasterController.ResetUsedMarkers(mrDuck.Name);
        mrDuck.ChaseCooldownTimer.Start(random.Next(5, 8));

        return MrDuckBehaviourStates.Roam;
    }

    private void ChaseCooldownTimer_OnTimeout()
    {
        idleTimer.Start(random.Next(5, 10));
    }
}