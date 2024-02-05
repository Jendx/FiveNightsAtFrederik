namespace FiveNightsAtFrederik.CsScripts.Interfaces;

public interface IStashable
{
    public bool IsDecayable { get; set; }
    public bool IsBreakable { get; set; }
    public bool IsUsed { get; set; }
}