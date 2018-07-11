using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public interface ISector
    {
        /// <summary>
        /// Точки выхода из сектора.
        /// </summary>
        IMapNode[] ExitNodes { get; set; }

        void Update();
    }
}