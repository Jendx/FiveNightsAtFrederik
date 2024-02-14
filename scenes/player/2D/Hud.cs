using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.scenes.player.Enums;
using Godot;
using Godot.Collections;
using System.Linq;

namespace FiveNightsAtFrederik.Scenes.Player;

public partial class Hud : Control
{
	private TextureRect crosshair;
	private ProgressBar staminaBar;
	private Player player;
	private StyleBoxFlat style;

	[Export]
	[ExportGroup("Dictionary<HudCrosshairStates, AudioStreamMp3> EnumValues: 0:Point, 1:Use, 2:Aim")]
	private Dictionary<HudCrosshairStates, Texture2D> defaultCrosshairTexture;

	public override void _Ready()
	{
		crosshair = this.TryGetNode<TextureRect>(NodeNames.Crosshair, nameof(crosshair));
		staminaBar = this.TryGetNode<ProgressBar>(NodeNames.StaminaBar, nameof(staminaBar));
		player = GetTree().GetNodesInGroup(GroupNames.playerGroup.ToString()).FirstOrDefault() as Player ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {GroupNames.playerGroup}");
		style = new StyleBoxFlat();

		player.OnPlayerUpdateCrosshairTexture += OnPlayerUpdateCrosshairTexture;
		staminaBar.Visible = false;
	}

	private void OnPlayerUpdateCrosshairTexture(HudCrosshairStates crosshairStates)
	{
		crosshair.Texture = defaultCrosshairTexture[crosshairStates];
	}

	public override void _Process(double delta)
	{
		staminaBar.Visible = player.CurrentStamina < (float)SprintThresholds.Max;
		style.BgColor = player.CurrentStamina switch
		{
			< (float)SprintThresholds.Low => new Color(0xe2433eff),
			< (float)SprintThresholds.Middle => new Color(0xff9c3cff),
			_ => new Color(0x898da2ff)
		};

		staminaBar.AddThemeStyleboxOverride("fill", style);
		staminaBar.Value = player.CurrentStamina;
	}
}




