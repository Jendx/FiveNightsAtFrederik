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
	private TextureRect? crosshair;
	private ProgressBar? staminaBar;
    private Player? player;
    private StyleBoxFlat? style;

	[Export]
	[ExportGroup("Dictionary<HudCrosshairStates, AudioStreamMp3> EnumValues: 0:Point, 1:Use, 2:Aim")]
	private Dictionary<HudCrosshairStates, Texture2D> defaultCrosshairTexture;

	public override void _Ready()
	{
		crosshair = this.TryGetNode<TextureRect>(NodeNames.Crosshair, nameof(crosshair));
		staminaBar = this.TryGetNode<ProgressBar>(NodeNames.StaminaBar, nameof(staminaBar));
		player = GetTree().GetNodesInGroup(GroupNames.PlayerGroup.ToString()).FirstOrDefault() as Player ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(player)} at {GroupNames.PlayerGroup}");
		style = new StyleBoxFlat();

		staminaBar.Visible = false;
	}

	public void UpdateCrosshairTexture(HudCrosshairStates crosshairStates)
	{
		crosshair.Texture = defaultCrosshairTexture[crosshairStates];
	}

	public override void _Process(double delta)
	{
		staminaBar.Visible = player.CurrentStamina < (float)SprintThresholds.Max;
		style.BgColor = player.CurrentStamina switch
		{
			< (float)SprintThresholds.Low => FrederikColors.Red,
			< (float)SprintThresholds.Middle => FrederikColors.Orange,
			_ => FrederikColors.White
        };

		staminaBar.AddThemeStyleboxOverride("fill", style);
		staminaBar.Value = player.CurrentStamina;
	}
}
