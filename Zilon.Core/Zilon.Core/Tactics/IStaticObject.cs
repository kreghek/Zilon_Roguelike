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
        /// Узер карты сектора, в котором находится контейнер.
        /// </summary>
        IGraphNode Node { get; }

        TSectorObjectModule GetModule<TSectorObjectModule>();

        void AddModule<TSectorObjectModule>(TSectorObjectModule sectorObjectModule);

        bool HasModule<TSectorObjectModule>();
    }
}
