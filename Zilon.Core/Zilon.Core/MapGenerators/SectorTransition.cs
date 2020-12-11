using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Info about transition to other sector.
    /// </summary>
    /// <remarks>
    /// Used to move user person between sector levels or biomes.
    /// </remarks>
    public sealed class SectorTransition
    {
        /// <summary>
        /// Создаение экземпляра перехода.
        /// </summary>
        public SectorTransition(ISectorNode sectorNode)
        {
            SectorNode = sectorNode ?? throw new System.ArgumentNullException(nameof(sectorNode));
        }

        /// <summary>
        /// Узел сектора в графе биома.
        /// </summary>
        public ISectorNode SectorNode { get; }

        /// <summary>
        /// Вывод строкого представления перехода.
        /// </summary>
        /// <returns>
        /// <see cref="string" />, который представляет переход.
        /// </returns>
        public override string ToString()
        {
            return SectorNode.ToString();
        }
    }
}