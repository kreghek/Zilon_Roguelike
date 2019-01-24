using System.Collections.Generic;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Деятель.
    /// </summary>
    public class Agent
    {
        public string Name { get; set; }

        /// <summary>
        /// Текущее местоположение деятеля.
        /// </summary>
        public TerrainCell Localtion { get; set; }

        /// <summary>
        /// Государтсво, на которое работает данный деятель.
        /// </summary>
        public Realm Realm { get; set; }

        public Dictionary<BranchType, int> Skills { get; set; }

        public override string ToString()
        {
            return $"{Name} [{Realm}] {Localtion}";
        }
    }
}
