using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Интерфейс тактического действия.
    /// </summary>
    public interface ITacticalAct
    {
        TacticalActScheme Scheme { get; }
    }
}
