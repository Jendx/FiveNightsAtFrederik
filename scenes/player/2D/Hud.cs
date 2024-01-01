using FiveNightsAtFrederik.CsScripts.Constants;
using Godot;

namespace FiveNightsAtFrederik.Scenes.Player;

public partial class Hud : Control
{
	private TextureRect crosshair;

	[Export]
	private Texture2D defaultCrosshairTexture;

	[Export]
	private Texture2D useCrosshairTexture;

	public override void _Ready()
	{
		crosshair = GetNode<TextureRect>(NodeNames.Crosshair.ToString());
		
	}

	private void OnPlayerUpdateCrosshairTexture(bool isUsableObject)
	{
		crosshair.Texture = isUsableObject ? useCrosshairTexture : defaultCrosshairTexture;
	}
}




