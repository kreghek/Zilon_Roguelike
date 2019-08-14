using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    /// <summary>
    /// Создание населённого пункта агентом.
    /// </summary>
    /// <remarks>
    /// Населённый пункт будет иметь специальность, близкую к специальности агента.
    /// </remarks>
    public class CreateLocality : IAgentCard
    {
        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            // Можем выполнять захват, если не превышен лимит городов текущего государства.
            var realmLocalityLimitReached = LocalityHelper.LimitIsReached(agent, globe);
            if (realmLocalityLimitReached)
            {
                return false;
            }

            // Создать город можно только из населения текущего нас.пункта.
            var hasCurrentLocality = globe.LocalitiesCells.TryGetValue(agent.Location, out var currentLocality);
            if (hasCurrentLocality)
            {
                if (currentLocality.Population >= 2)
                {
                    return true;
                }
            }

            return false;
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            globe.LocalitiesCells.TryGetValue(agent.Location, out var currentLocality);

            var highestBranchs = agent.Skills
                                    .Where(x => /*x.Key != BranchType.Politics &&*/ x.Value >= 1)
                                    .OrderBy(x => x.Value);

            if (!highestBranchs.Any())
            {
                return;
            }

            var firstBranch = highestBranchs.First();


            // Обнаружение свободных узлов для размещения населённого пункта.
            // Свободные узлы ишутся от текущей локации агента.

            TerrainCell freeLocaltion = null;

            var nextCoords = HexHelper.GetOffsetClockwise();
            var agentCubeCoords = HexHelper.ConvertToCube(agent.Location.Coords.X, agent.Location.Coords.Y);
            for (var i = 0; i < nextCoords.Length; i++)
            {
                var scanCubeCoords = agentCubeCoords + nextCoords[i];
                var scanOffsetCoords = HexHelper.ConvertToOffset(scanCubeCoords);

                var freeX = scanOffsetCoords.X;
                var freeY = scanOffsetCoords.Y;

                // Убеждаемся, что проверяемый узел находится в границах мира.
                var isPointInside = IsPointInsideWorld(freeX, freeY, globe.Terrain);
                if (!isPointInside)
                {
                    continue;
                }

                // Проверка, есть ли в найденной локации населённые пункты.
                var freeLocaltion1 = globe.Terrain[freeX][freeY];

                if (!globe.LocalitiesCells.TryGetValue(freeLocaltion1, out var freeCheckLocality))
                {
                    freeLocaltion = globe.Terrain[freeX][freeY];
                }
            }

            if (freeLocaltion != null)
            {
                // Свободный узел был найден.
                // Тогда создаём здесь населённый пункт с доминирующей специаьностью агента.
                // Популяция нового нас.пункта минимальна.
                // Одна единица популяци из текущего нас.пункта снимается.
                // Считается, что часть жителей мигрировали для начала строительства нового нас.пункта.

                var localityName = globe.GetLocalityName(dice);

                var createdLocality = new Locality
                {
                    Name = localityName,
                    Branches = new Dictionary<BranchType, int> { { firstBranch.Key, 1 } },
                    Cell = freeLocaltion,

                    Population = 1,
                    Owner = currentLocality.Owner
                };

                currentLocality.Population--;

                globe.Localities.Add(createdLocality);
                globe.LocalitiesCells[freeLocaltion] = createdLocality;
                globe.ScanResult.Free.Remove(freeLocaltion);
            }
            else
            {
                // Если не удалось найти свободный узел,
                // то агент перемещается в произвольный населённый пункт своего государства.

                TransportHelper.TransportAgentToRandomLocality(globe, dice, agent, currentLocality);
            }
        }

        private static bool IsPointInsideWorld(int freeX, int freeY, TerrainCell[][] terrain)
        {
            if (freeX < 0)
            {
                return false;
            }

            if (freeX >= terrain.Length)
            {
                return false;
            }

            if (freeY < 0)
            {
                return false;
            }

            if (freeY >= terrain[freeX].Length)
            {
                return false;
            }

            return true;
        }
    }
}
