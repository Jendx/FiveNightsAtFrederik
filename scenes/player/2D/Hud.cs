using FiveNightsAtFrederik.Constants;
using FiveNightsAtFrederik.CsScripts.Controllers;
using Godot;

namespace FiveNightsAtFrederik.Scenes.Player;

public partial class Hud : Control
{
	private TextureRect _crosshair;

	[Export]
	private Texture2D _defaultCrosshairTexture;

	[Export]
	private Texture2D _useCrosshairTexture;

	public override void _Ready()
	{
		_crosshair = GetNode<TextureRect>(NodeNames.Crosshair.ToString());
		
	}

	private void OnPlayerUpdateCrosshairTexture(bool isUsableObject)
	{
		_crosshair.Texture = isUsableObject ? _useCrosshairTexture : _defaultCrosshairTexture;
	}
}




