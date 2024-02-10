using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.scenes.player.Enums;
using FiveNightsAtFrederik.Scenes.Player._3D.Gun.Enums;
using Godot;
using Godot.Collections;
using System.Threading.Tasks;

namespace FiveNightsAtFrederik.Scenes.Player;

public partial class Gun : BaseHoldableItem, IAnimated<PlayerAnimationStates?>
{
	[Export]
	private const float reloadTime = 1.2f;

	// Cooldown between shots
	[Export]
	private const float fireRate = 0.2f;

	[Export]
	private const float automaticReloadDelay = 0.9f;

	[Export]
	[ExportGroup("Dictionary<GunSounds, AudioStreamMp3> EnumValues: 0:Shoot, 1:Reload")]
	private Dictionary<GunSounds, AudioStreamMP3> gunSounds;
	[ExportGroup("")]

	private AudioStreamPlayer3D audioPlayer;
	private RayCast3D rayCast;
	private Timer fireCooldownTimer;
	private Timer automaticReloadTimer;
	private bool isOnFireCooldown;
	private bool isLoaded = true;
    private bool isReloading; 


    public override void _Ready()
	{
		base._Ready();
		audioPlayer = this.TryGetNode<AudioStreamPlayer3D>(NodeNames.AudioPlayer.ToString(), nameof(audioPlayer));
		rayCast = this.TryGetNode<RayCast3D>(NodeNames.RayCast, nameof(rayCast));
		fireCooldownTimer = this.TryGetNode<Timer>(NodeNames.DelayTimer, nameof(fireCooldownTimer));
		fireCooldownTimer.Timeout += () => isOnFireCooldown = false;
		automaticReloadTimer = this.TryGetNode<Timer>(NodeNames.AutomaticReloadTimer, nameof(automaticReloadDelay));
		automaticReloadTimer.Timeout += async () =>
		{
			if (IsHeld)
			{
				await Reload();
			};
		};
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
	/// Unparents gun from player if player is not currently reloading
	/// </summary>
	protected override void Drop()
	{
		if (!(IsHeld && Input.IsActionJustPressed(ActionNames.Drop)) || isReloading)
		{
			return;
		}

		// Stop automaticReloadTimer so when player drops the gun, it is not reloaded
		automaticReloadTimer.Stop();

		Reparent(originalParent);
		IsHeld = false;
		Freeze = false;
		SetCollisionLayerValue((int)CollisionLayers.PlayerCollideable, true);
	}

	/// <summary>
	/// Fires weapon with fireRate. Also automatically reloads if player keeps holding the gun
	/// </summary>
	private void Fire()
	{
		if (!(IsHeld && Input.IsActionJustPressed(ActionNames.Use)) || isOnFireCooldown)
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

	private async Task Reload()
	{
        // TODO: Add & Play reload Animation
        isReloading = true;
		audioPlayer.PlayStream(gunSounds[GunSounds.Reload]); 	
		var timer = GetTree().CreateTimer(reloadTime);
		await ToSignal(timer, "timeout");

        isReloading = false;
		isLoaded = true;
		return;
	}

	public override void OnBeginUse()
	{
		Freeze = true;
		Reparent(player.EquipableItemPositionMarker);

		GlobalPosition = player.EquipableItemPositionMarker.GlobalPosition;
		Rotation = Vector3.Zero;
		player.IsHoldingItem = true;
		IsHeld = true;
		SetCollisionLayerValue((int)CollisionLayers.PlayerCollideable, false);
	}

    public PlayerAnimationStates? HandleAnimations()
    {
        if (isReloading)
        {
            return PlayerAnimationStates.Reload;
        }

        if (IsHeld)
        {
            return PlayerAnimationStates.IdleArmed;
        }

        return null;
    }
}
