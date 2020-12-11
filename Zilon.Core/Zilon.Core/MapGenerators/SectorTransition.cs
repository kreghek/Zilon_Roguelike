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
        /// Constructor.
        /// </summary>
        /// <param name="sectorNode">The node of the globe graph with next sector.</param>
        public SectorTransition(ISectorNode sectorNode)
        {
            SectorNode = sectorNode ?? throw new System.ArgumentNullException(nameof(sectorNode));
        }

        /// <summary>
        /// The node of the globe graph with next sector.
        /// </summary>
        public ISectorNode SectorNode { get; }

        public override string ToString()
        {
            return $"Transition to {SectorNode}";
        }
    }
}