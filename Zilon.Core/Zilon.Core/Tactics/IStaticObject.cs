using Zilon.Core.Graphs;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public interface IStaticObject: IPassMapBlocker
    {
        /// <summary>
        /// Идентфикиатор объекта.
        /// </summary>
        /// <remarks>
        /// Задаётся и используется в тестах для выборки сундука.
        /// </remarks>
        int Id { get; }

        /// <summary>
        /// Данные о содержимом статического объекта.
        /// </summary>
        IPropContainer PropContainer { get; }

        /// <summary>
        /// Узер карты сектора, в котором находится контейнер.
        /// </summary>
        IGraphNode Node { get; }
    }
}
