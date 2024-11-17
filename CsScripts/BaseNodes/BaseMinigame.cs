using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;
using System.Linq;
using System.Threading.Tasks;

namespace FiveNightsAtFrederik.CsScripts.BaseNodes;

public abstract partial class BaseMinigame : Node3D, IPlayerUsable
{
    public bool IsInteractionUIDisplayed { get; set; } = true;

    protected bool isActive;
    protected Player? player;
    protected Camera3D? minigameCamera;
    protected CollisionShape3D? interactionCollision;
    protected Control? minigameHud;

    public override void _Ready()
    {
        player = GetTree().GetNodesInGroup(GroupNames.PlayerGroup.ToString()).FirstOrDefault() as Player ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {NodeNames.PlayerInRoot}");
        minigameCamera = this.TryGetNode<Camera3D>(NodeNames.MinigameCamera, nameof(minigameCamera));
        interactionCollision = this.TryGetNode<CollisionShape3D>(NodeNames.MinigameInteractionCollision, nameof(interactionCollision));
        minigameHud = this.TryGetNode<Control>(NodeNames.MinigameHud, nameof(minigameHud));
        minigameHud.Hide();
    }

    /// <summary>
    /// Disables input for minigame and returns camera to player
    /// </summary>
    protected virtual void LeaveMinigame()
    {
        isActive = false;
        player.IsPlayingMinigame = false;
        minigameCamera.Current = false;

        player.Camera.Current = true;
        minigameHud.Hide();
        player.ShowHud();
    }

    protected abstract void ResetMinigame();
    protected abstract void FinishMinigame();

    public virtual void OnBeginUse()
    {
        player.IsPlayingMinigame = true;
        minigameCamera.Current = true;

        player.Camera.Current = false;
        _ = ActivateMinigame();

        minigameHud.Show();
        player.HideHud();
    }

    public virtual void OnEndUse()
    {
    }

    /// <summary>
    /// Activates minigame after small offset So there is no misinput
    /// </summary>
    /// <returns></returns>
    protected async Task ActivateMinigame()
    {
        var timer = GetTree().CreateTimer(0.3f);
        await ToSignal(timer, "timeout");

        isActive = true;
    }
}
