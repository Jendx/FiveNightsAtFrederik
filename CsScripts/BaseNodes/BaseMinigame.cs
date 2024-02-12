using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;
using System.Linq;

namespace FiveNightsAtFrederik.CsScripts.BaseNodes;

public abstract partial class BaseMinigame : Node3D
{
    protected bool isActive = false;
    protected Player player;
    protected Camera3D minigameCamera;
    protected CollisionShape3D interactionCollision;

    protected BaseMinigame()
    {
    }

    public override void _Ready()
    {
        player = GetTree().GetNodesInGroup(GroupNames.playerGroup.ToString()).FirstOrDefault() as Player ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {NodeNames.PlayerInRoot}");
        minigameCamera = this.TryGetNode<Camera3D>(NodeNames.MinigameCamera, nameof(minigameCamera));
        interactionCollision = this.TryGetNode<CollisionShape3D>(NodeNames.MinigameInteractionCollision, nameof(interactionCollision));
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
    }

    protected abstract void ResetMinigame();
    protected abstract void TryWin();

    public void OnBeginUse()
    {
        isActive = true;
        player.IsPlayingMinigame = true;
        minigameCamera.Current = true;

        player.Camera.Current = false;
    }

    public void OnEndUse()
    {
    }
}
