namespace FiveNightsAtFrederik.CsScripts.Interfaces;

public interface IStashable
{
    public bool IsStashed { get; set; }
    public void Stash();
}