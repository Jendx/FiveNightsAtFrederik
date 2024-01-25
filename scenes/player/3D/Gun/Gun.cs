using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.Scenes.player._3D.Gun.Enums;
using Godot;
using Godot.Collections;

namespace FiveNightsAtFrederik.Scenes.Player;

public partial class Gun : RigidBody3D, IPlayerUsable
{
    [Export]
    public bool IsInteractionUIDisplayed { get; set; } = true;

    [Export]
    [ExportGroup("Dictionary<GunSounds, AudioStreamMp3> EnumValues: 0:Shoot, 1:Reload")]
    private Dictionary<GunSounds, AudioStreamMP3> gunSounds;
    [ExportGroup("")]

    private Player player;
    private AudioStreamPlayer3D audioPlayer;
    private RayCast3D rayCast;
    private Node originalParent;
    private bool isHeld;

    public override void _Ready()
    {
        player = GetNode<Player>(NodeNames.PlayerInRoot.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {NodeNames.PlayerInRoot}"); ;
        audioPlayer = GetNode<AudioStreamPlayer3D>(NodeNames.AudioPlayer.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(audioPlayer)} at {NodeNames.AudioPlayer}"); ;
        rayCast = GetNode<RayCast3D>(NodeNames.RayCast.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(rayCast)} at {NodeNames.RayCast}"); ;
        
        originalParent = GetParent();
    }

    public override void _Input(InputEvent @event)
    {
        if (isHeld && Input.IsActionJustPressed(ActionNames.Drop))
        {
            Reparent(originalParent);
            player.IsHoldingWeapon = false;
            Freeze = false;
        }

        if (isHeld && Input.IsActionJustPressed(ActionNames.Use))
        {
            Fire();
        }
    }

    private void Fire()
    {
        audioPlayer.Stream = gunSounds[GunSounds.Shoot];
        audioPlayer.Play();

        var collidingObject = rayCast.GetCollider();

        var shotObject = collidingObject is IDamagable ? (IDamagable)collidingObject : ((Node)collidingObject).Owner as IDamagable;
        shotObject?.ApplyDamage();
    }

    public void OnBeginUse()
    {
        Freeze = true;
        Reparent(player.EquipableItemPositionMarker);

        GlobalPosition = player.EquipableItemPositionMarker.GlobalPosition;
        Rotation = player.EquipableItemPositionMarker.Rotation;

        player.IsHoldingWeapon = true;
        isHeld = true;
    }

    public void OnEndUse()
    {
    }
}
