using System.Collections.Generic;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Мир.
    /// </summary>
    public class Globe
    {
        public TerrainCell[][] Terrain { get; set; }
        public List<Realm> Realms = new List<Realm>();
        public List<Locality> localities = new List<Locality>();
        public List<Agent> agents = new List<Agent>();
        public Dictionary<TerrainCell, List<Agent>> agentCells = new Dictionary<TerrainCell, List<Agent>>();
        public Dictionary<TerrainCell, Locality> localitiesCells = new Dictionary<TerrainCell, Locality>();
    }
}
