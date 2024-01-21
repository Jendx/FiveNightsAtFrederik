namespace FiveNightsAtFrederik.CsScripts.Interfaces;

public interface IPlayerUsable
{
    /// <summary>
    /// Determines if the player's crosshair should change to Useable Icon (Hand) when player is looking at IPlayerUsable Object
    /// </summary>
    public bool isInteractionUIDisplayed { get; set; }

    public void OnBeginUse();
    public void OnEndUse();
}
