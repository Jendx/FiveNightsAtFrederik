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
	public AnimationTree AnimationTree { get; private set; }

	[Export]
	private float reloadTime = 3.3f;

	// Cooldown between shots
	[Export]
	private float fireRate = 0.2f;

	[Export]
	private float automaticReloadDelay = 0.9f;

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
	private bool isShooting;
	private GunAnimationStates currentAnimation = GunAnimationStates.Idle;
	private GunAnimationStates nextAnimation;

	public override void _Ready()
	{
		base._Ready();
		audioPlayer = this.TryGetNode<AudioStreamPlayer3D>(NodeNames.AudioPlayer.ToString(), nameof(audioPlayer));
		rayCast = this.TryGetNode<RayCast3D>(NodeNames.RayCast, nameof(rayCast));
		fireCooldownTimer = this.TryGetNode<Timer>(NodeNames.DelayTimer, nameof(fireCooldownTimer));
		AnimationTree = this.TryGetNode<AnimationTree>(NodeNames.AnimationTree, nameof(AnimationTree));
		fireCooldownTimer.Timeout += () =>
		{
			isOnFireCooldown = false;
			isShooting = false;
		};

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
		IsInteractionUIDisplayed = true;
		Reparent(originalParent);
		IsHeld = false;
		Freeze = false;
		SetCollisionLayerValue((int)CollisionLayers.PlayerCollideable, true);
		player.IsHoldingItem = false;
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
		isShooting = true;
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
		player.UpdateCrosshairState(HudCrosshairStates.Aim);
		IsInteractionUIDisplayed = false;
		if(player.IsCarryingItem)
		{
			return;
		}
		Freeze = true;
		Reparent(player.EquipableItemPositionMarker);

		GlobalPosition = player.EquipableItemPositionMarker.GlobalPosition;
		Rotation = Vector3.Zero;
		player.IsHoldingItem = true;
		IsHeld = true;
		SetCollisionLayerValue((int)CollisionLayers.PlayerCollideable, false);
	}

	public void UpdateGunAnimation()
	{
		if (currentAnimation == nextAnimation)
		{
			return;
		}

		AnimationTree.Set(currentAnimation.GetDescription(), false);

		currentAnimation = nextAnimation;

		AnimationTree.Set(currentAnimation.GetDescription(), true);
	}

	public PlayerAnimationStates? HandleAnimations()
	{
		if (isReloading)
		{
			nextAnimation = GunAnimationStates.Reload;
			UpdateGunAnimation();
			return PlayerAnimationStates.Reload;
		}

		if (isShooting)
		{
			nextAnimation = GunAnimationStates.Shoot;
			UpdateGunAnimation();
			return PlayerAnimationStates.Shoot;
		}

		if (IsHeld)
		{
			nextAnimation = GunAnimationStates.Idle;
			UpdateGunAnimation();
			return PlayerAnimationStates.IdleArmed;
		}
		
		return null;
	}
}
