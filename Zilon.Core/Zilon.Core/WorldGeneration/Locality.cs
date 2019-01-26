using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Город.
    /// </summary>
    public class Locality
    {
        public string Name { get; set; }

        public TerrainCell Cell { get; set; }

        public Realm Owner { get; set; }

        public Dictionary<BranchType, int> Branches { get; set; }

        public int Population { get; set; }

        public override string ToString()
        {
            return $"{Name} [{Owner}] ({Branches.First().Key})";
        }
    }
}
