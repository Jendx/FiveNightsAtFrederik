namespace FiveNightsAtFrederik.CsScripts.Interfaces;

public interface IUsable
{
    public void OnBeginUse(bool isToggle);
    public void OnEndUse(bool isToggle);
}
