using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using Godot;
using Godot.Collections;

namespace FiveNightsAtFrederik.Scenes.Player;

public partial class Hud : Control
{
	private TextureRect crosshair;

	[Export]
    [ExportGroup("Dictionary<HudCrosshairStates, AudioStreamMp3> EnumValues: 0:Point, 1:Use, 2:Aim")]
    private Dictionary<HudCrosshairStates, Texture2D> defaultCrosshairTexture;

    public override void _Ready()
	{
		crosshair = GetNode<TextureRect>(NodeNames.Crosshair.ToString());
	}

	private void OnPlayerUpdateCrosshairTexture(HudCrosshairStates crosshairStates)
	{
        crosshair.Texture = defaultCrosshairTexture[crosshairStates];
    }
}




