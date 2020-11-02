using Zilon.Core.Graphs;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    /// <summary>
    /// Узел с сектором в графе биома.
    /// </summary>
    public interface ISectorNode : IGraphNode
    {
        /// <summary>
        /// Биом, частью которого является узел графа.
        /// </summary>
        IBiome Biome { get; }

        /// <summary>
        /// Материализованный сектор, если он есть.
        /// </summary>
        ISector Sector { get; }

        /// <summary>
        /// Схема, по которой был материализован сектор.
        /// </summary>
        ISectorSubScheme SectorScheme { get; }

        /// <summary>
        /// Состояние узла графа биома.
        /// </summary>
        SectorNodeState State { get; }

        /// <summary>
        /// Указать информацию о схемах для последующей материализации сектора.
        /// </summary>
        /// <param name="biom"></param>
        /// <param name="sectorScheme"></param>
        void BindSchemeInfo(IBiome biom, ISectorSubScheme sectorScheme);

        /// <summary>
        /// Материализация сетора.
        /// </summary>
        /// <param name="sector"></param>
        void MaterializeSector(ISector sector);
    }
}