using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.Scenes.player._3D.Gun.Enums;
using Godot;
using Godot.Collections;
using System.Threading.Tasks;

namespace FiveNightsAtFrederik.Scenes.Player;

public partial class Gun : RigidBody3D, IPlayerUsable
{
    [Export]
    public bool IsInteractionUIDisplayed { get; set; } = true;

    [Export]
    private const int reloadTime = 1;

    // Cooldown between shots
    [Export]
    private const float fireRate = 0.2f;

    [Export]
    private const float automaticReloadDelay = 0.9f;

    [Export]
    [ExportGroup("Dictionary<GunSounds, AudioStreamMp3> EnumValues: 0:Shoot, 1:Reload")]
    private Dictionary<GunSounds, AudioStreamMP3> gunSounds;
    [ExportGroup("")]

    private Player player;
    private AudioStreamPlayer3D audioPlayer;
    private RayCast3D rayCast;
    private Timer fireCooldownTimer;
    private Timer automaticReloadTimer;
    private Node originalParent;

    private bool isHeld;
    private bool isOnFireCooldown;
    private bool isLoaded = true;
    private bool isReloading;

    public override void _Ready()
    {
        player = GetNode<Player>(NodeNames.PlayerInRoot.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {NodeNames.PlayerInRoot}"); ;
        audioPlayer = GetNode<AudioStreamPlayer3D>(NodeNames.AudioPlayer.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(audioPlayer)} at {NodeNames.AudioPlayer}"); ;
        rayCast = GetNode<RayCast3D>(NodeNames.RayCast.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(rayCast)} at {NodeNames.RayCast}"); ;
        fireCooldownTimer = GetNode<Timer>(NodeNames.DelayTimer.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(fireCooldownTimer)} at {NodeNames.DelayTimer}"); ;
        fireCooldownTimer.Timeout += () => isOnFireCooldown = false;

        automaticReloadTimer = GetNode<Timer>(NodeNames.AutomaticReloadTimer.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(automaticReloadDelay)} at {NodeNames.AutomaticReloadTimer}"); ;
        automaticReloadTimer.Timeout += async () =>
        {
            if (isHeld)
            {
                await Reload();
            };
        };

        originalParent = GetParent();
    }

    public override void _Input(InputEvent @event)
    {
        if (isReloading)
        {
            return;
        }

        Drop();
        Fire();
    }

    /// <summary>
    /// Fires weapon with fireRate. Also automatically reloads if player keeps holding the gun
    /// </summary>
    private void Fire()
    {
        if (!(isHeld && Input.IsActionJustPressed(ActionNames.Use)) || isOnFireCooldown)
        {
            return;
        }

        audioPlayer.Stream = gunSounds[isLoaded ? GunSounds.Shoot : GunSounds.ShootEmpty];
        audioPlayer.Play();

        // If loaded, fire weapon & start reloading if player keeps holding gun
        if (isLoaded)
        {
            isLoaded = false;
            var collidingObject = rayCast.GetCollider();

            var shotObject = collidingObject is IDamagable ? (IDamagable)collidingObject : ((Node)collidingObject)?.Owner as IDamagable;
            shotObject?.ApplyDamage();
        }

        isOnFireCooldown = true;
        fireCooldownTimer.Start(fireRate);
        // after little delay reload gun, if it is still held
        automaticReloadTimer.Start(automaticReloadDelay);
    }

    /// <summary>
    /// Unparents gun from player if player is not currently reloading
    /// </summary>
    private void Drop()
    {
        if (!(isHeld && Input.IsActionJustPressed(ActionNames.Drop)) || isReloading)
        {
            return;
        }

        // Stop automaticReloadTimer so when player drops the gun, it is not reloaded
        automaticReloadTimer.Stop();

        Reparent(originalParent);
        player.IsHoldingWeapon = false;
        isHeld = false;
        Freeze = false;
        SetCollisionLayerValue((int)CollisionLayers.PlayerCollideable, true);
    }

    private async Task Reload()
    {
        // TODO: Add & Play reload Animation

        audioPlayer.Stream = gunSounds[GunSounds.Reload];
        audioPlayer.Play();

        isReloading = true;
        var timer = GetTree().CreateTimer(reloadTime);
        await ToSignal(timer, "timeout");
        isReloading = false;

        isLoaded = true;
        return;
    }

    /// <summary>
    /// Attaches gun to player & makes it not collidible with player
    /// </summary>
    public void OnBeginUse()
    {
        Freeze = true;
        Reparent(player.EquipableItemPositionMarker);

        GlobalPosition = player.EquipableItemPositionMarker.GlobalPosition;
        Rotation = player.EquipableItemPositionMarker.Rotation;

        player.IsHoldingWeapon = true;
        isHeld = true;
        SetCollisionLayerValue((int)CollisionLayers.PlayerCollideable, false);
    }

    public void OnEndUse() {}
}
