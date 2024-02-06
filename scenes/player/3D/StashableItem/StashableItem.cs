using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using System.Linq;

namespace FiveNightsAtFrederik.Scenes.Player;

[GlobalClass]
public partial class StashableItem : BaseCarriableItem, IStashable
{
	[Export]
	public bool IsDecayable { get; set; }

	[Export]
	public bool IsBreakable { get; set; }

	[Export]
	public bool IsUsed { get; set; }
}
