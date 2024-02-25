using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Controllers;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using Godot.Collections;
using System;

namespace FiveNightsAtFrederik.scenes.level.Minigames.TakeOrder;

public partial class TakeOrderMinigame : Node3D, IPlayerUsable
{
    public bool IsInteractionUIDisplayed { get; set; }

    [Export]
    private AudioStreamMP3 Ringtone;

    [Export]
    private Dictionary<PizzaType, AudioStreamMP3> orderRecordings = new();
   
    private AudioStreamPlayer3D audioPlayer;
    private readonly Random random = new();
    private bool isRinging;

    public override void _Ready()
    {
        audioPlayer = this.TryGetNode<AudioStreamPlayer3D>(NodeNames.AudioPlayer, nameof(audioPlayer));
        OrderController.OnOrderCreated += StartRinging;

        var timer = GetTree().CreateTimer(random.Next(3, 8));
        timer.Timeout += () => OrderController.CreateOrder();
    }

    public void StartRinging()
    {
        audioPlayer.MaxDistance = 40;
        audioPlayer.PlayStream(Ringtone);
        isRinging = true;
        IsInteractionUIDisplayed = true;
    }

    public void OnBeginUse()
    {
        if (!isRinging)
        {
            return;
        }

        isRinging = false;
        IsInteractionUIDisplayed = false;
        audioPlayer.MaxDistance = 20;
        audioPlayer.PlayStream(orderRecordings[OrderController.CurrentOrder]);
    }

    public void OnEndUse() {}
}
