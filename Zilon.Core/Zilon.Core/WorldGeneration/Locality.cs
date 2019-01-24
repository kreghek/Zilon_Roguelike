using System.Collections.Generic;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Город.
    /// </summary>
    public class Locality
    {
        public string Name { get; set; }

        public TerrainCell[] Cells { get; set; }

        public Realm Owner { get; set; }

        public Dictionary<BranchType, int> Branches { get; set; }

        public int Population { get; set; }
    }
}
