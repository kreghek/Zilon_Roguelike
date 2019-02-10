using Zilon.Core.Persons;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.Players
{
    public class HumanPlayer: PlayerBase
    {
        public TerrainCell Terrain { get; set; }
        public GlobeRegionNode GlobeNode { get; set; }
        public HumanPerson MainPerson { get; set; }
    }
}
