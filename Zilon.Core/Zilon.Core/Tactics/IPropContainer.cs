using System;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Интерфейт предмета в секторе.
    /// </summary>
    public interface IPropContainer
    {
        /// <summary>
        /// Узер карты сектора, в котором находится контейнер.
        /// </summary>
        IMapNode Node { get; }
    }
}
