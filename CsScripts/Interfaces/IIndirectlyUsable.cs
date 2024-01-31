using FiveNightsAtFrederik.CsScripts.Models.Interfaces;

namespace FiveNightsAtFrederik.CsScripts.Interfaces;

/// <summary>
/// Objects that inherit this Interface should respond to IPlayerUsable object calls
/// </summary>
public interface IIndirectlyUsable<TModel> where TModel : BaseUsableParameters
{
    public void OnBeginUse(TModel? parameters);
    public void OnEndUse(TModel? parameters);
}
