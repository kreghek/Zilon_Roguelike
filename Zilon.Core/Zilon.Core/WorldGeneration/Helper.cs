using System.Collections.Generic;

namespace Zilon.Core.WorldGeneration
{
    public static class Helper
    {
        public static void AddAgentToCell(Dictionary<TerrainCell, List<Agent>> cells, TerrainCell cell, Agent agent)
        {
            if (cells.TryGetValue(cell, out var list))
            {
                list.Add(agent);
            }
            else
            {
                list = new List<Agent> { agent };
                cells[cell] = list;
            }
        }

        public static void RemoveAgentFromCell(Dictionary<TerrainCell, List<Agent>> cells, TerrainCell cell, Agent agent)
        {
            if (cells.TryGetValue(cell, out var list))
            {
                list.Remove(agent);
            }
        }
    }
}
