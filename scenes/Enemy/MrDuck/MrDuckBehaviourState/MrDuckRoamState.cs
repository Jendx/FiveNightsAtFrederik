using FiveNightsAtFrederik.CsScripts.Controllers;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourFactory.Abstraction;
using FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourState.Enums;
using FiveNightsAtFrederik.Scenes.Enemy;
using Godot;
using Godot.Collections;
using System;

namespace FiveNightsAtFrederik.scenes.Enemy.MrDuck.BehaviourFactory;

public class MrDuckRoamState : MrDuckBaseState
{
    private const int roamTargetDesiredDistance = 1;
    private readonly EnemyMasterController controller;

    public MrDuckRoamState(
        Scenes.Enemy.MrDuck mrDuck,
        AudioStreamPlayer3D audioPlayer,
        Random random,
        Dictionary<EnemySounds, AudioStreamMP3> audioTracks,
        Timer idleTimer) : base(idleTimer, audioTracks, random, audioPlayer, mrDuck)
    {
        controller = new EnemyMasterController(mrDuck);
    }

    public override void HandleBehaviour()
    {
        if (mrDuck.NavigationAgent.TargetDesiredDistance > roamTargetDesiredDistance)
        {
            mrDuck.NavigationAgent.TargetDesiredDistance = roamTargetDesiredDistance;
            HandleTargetReached();
        }

        if (TryDeactivateDuck(audioTracks[EnemySounds.Deactivate]))
        {
            return;
        }

        ActivateDuck(audioTracks[EnemySounds.Activate]);
    }

    public void ActivateDuck(AudioStreamMP3 activationAudio)
    {
        if (!mrDuck.IsActive)
        {
            audioPlayer.PlayStream(activationAudio);
        }

        mrDuck.IsActive = true;
        GD.Print("Duck Activated");
    }

    /// <summary>
    /// If the random returns number > 3 the duck will deactivate 
    /// </summary>
    /// <param name="deactivationAudio"></param>
    /// <param name="isActive"></param>
    /// <returns>True, if duck is deactivated. False if duck didn't got lucky</returns>
    public bool TryDeactivateDuck(AudioStreamMP3 deactivationAudio)
    {
        var number = random.Next(11);
        if (number <= 3)
        {
            idleTimer.Start(random.Next(10, 20));
            return false;
        }

        if (mrDuck.IsActive)
        {
            audioPlayer.PlayStream(deactivationAudio);
        }

        GD.Print("Duck Deactivated");
        mrDuck.IsActive = false;
        return true;
    }

    /// <summary>
    /// When duck gets to final destination. Just get next available position
    /// </summary>
    public override MrDuckBehaviourStates HandleTargetReached()
    {
        mrDuck.CurrentDestinationMarker = controller.GetNextPossibleDestination(mrDuck.Name);
        mrDuck.NavigationAgent.TargetPosition = mrDuck.CurrentDestinationMarker.GlobalPosition;

        GD.Print($"Target Location switched to: {mrDuck.CurrentDestinationMarker.Name}");

        return MrDuckBehaviourStates.Roam;
    }
}
