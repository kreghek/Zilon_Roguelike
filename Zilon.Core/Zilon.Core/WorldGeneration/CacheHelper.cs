using System.Collections.Generic;
using System.Linq;
using Zilon.Core.World;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Вспомогательный класс для работы с кешем агентов.
    /// Этот класс намеренно не проверяет наличие ключей в словаре. Метод PrepareDict гарантирует
    /// наличие значений по всем возможным ключам.
    /// </summary>
    public static class CacheHelper
    {
        public static void AddAgentToCell(Dictionary<TerrainCell, List<Agent>> cellDict, TerrainCell cell, Agent agent)
        {
            if (cellDict.TryGetValue(cell, out var list))
            {
                list.Add(agent);
            }
            else
            {
                list = new List<Agent> { agent };
                cellDict[cell] = list;
            }
        }

        public static void RemoveAgentFromCell(Dictionary<TerrainCell, List<Agent>> cellDict, TerrainCell cell, Agent agent)
        {
            if (cellDict.TryGetValue(cell, out var list))
            {
                list.Remove(agent);

                if (!list.Any())
                {
                    cellDict.Remove(cell);
                }
            }
        }
    }
}
