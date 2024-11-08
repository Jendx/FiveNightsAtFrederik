using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Controllers;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using System;
using System.Threading.Tasks;

namespace FiveNightsAtFrederik.scenes.level.Minigames.TakeOrder;

public partial class TakeOrderMinigame : Node3D, IPlayerUsable
{
    public bool IsInteractionUIDisplayed { get; set; }

    [Export]
    private AudioStreamMP3 Ringtone;
   
    private readonly Random random = new();
    private AudioStreamPlayer3D audioPlayer;
    private bool isRinging;

    public override void _Ready()
    {
        audioPlayer = this.TryGetNode<AudioStreamPlayer3D>(NodeNames.AudioPlayer, nameof(audioPlayer));

        OrderController.OnOrderCreated += () => _ = StartRinging();
        OrderController.CreateOrder();
    }

    /// <summary>
    /// Plays ringtone with little delay until player interacts with telephone
    /// </summary>
    /// <returns></returns>
    public async Task StartRinging()
    {
        var timer = GetTree().CreateTimer(random.Next(3, 8));
        await ToSignal(timer, "timeout");

        audioPlayer.MaxDistance = 40;
        audioPlayer.PlayStream(Ringtone);
        isRinging = true;
        IsInteractionUIDisplayed = true;
    }

    /// <summary>
    /// Plays order audio
    /// </summary>
    public void OnBeginUse()
    {
        if (!isRinging)
        {
            return;
        }

        isRinging = false;
        IsInteractionUIDisplayed = false;
        audioPlayer.MaxDistance = 20;
        audioPlayer.PlayStream(OrderController.CurrentOrder.OrderAudioStream);
    }

    public void OnEndUse() {}
}
