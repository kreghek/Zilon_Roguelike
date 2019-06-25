using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Вспомогательный класс для работы с кешем агентов.
    /// Этот класс намеренно не проверяет наличие ключей в словаре. Метод PrepareDict гарантирует
    /// наличие значений по всем возможным ключам.
    /// </summary>
    public static class CacheHelper
    {
        public static void PrepareDict(Dictionary<TerrainCell, List<Agent>> cellDict, TerrainCell[][] cells)
        {
            foreach (var cellRow in cells)
            {
                foreach (var cell in cellRow)
                {
                    cellDict.Add(cell, new List<Agent>());
                }
            }
        }

        public static void ClearDict(Dictionary<TerrainCell, List<Agent>> cellDict)
        {
            var cellKeys = cellDict.Keys.ToArray();
            foreach (var cell in cellKeys)
            {
                var list = cellDict[cell];
                if (!list.Any())
                {
                    cellDict.Remove(cell);
                }
            }
        }


        public static void AddAgentToCell(Dictionary<TerrainCell, List<Agent>> cellDict, TerrainCell cell, Agent agent)
        {
            var list = cellDict[cell];
            list.Add(agent);
        }

        public static void RemoveAgentFromCell(Dictionary<TerrainCell, List<Agent>> cellDict, TerrainCell cell, Agent agent)
        {
            var list = cellDict[cell];
            list.Remove(agent);
        }
    }
}
