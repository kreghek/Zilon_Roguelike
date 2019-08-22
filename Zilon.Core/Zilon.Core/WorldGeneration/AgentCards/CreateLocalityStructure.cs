using System;
using System.Linq;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    /// <summary>
    /// Глава города принимает решение начать строительство нового здания.
    /// </summary>
    public sealed class CreateLocalityStructure : IAgentCard
    {
        /// <summary>
        /// Это максимальная вместимость районов.
        /// Столько структур можно разместить в одном районе.
        /// </summary>
        private const int MAX_REGION_CAPABILITY = 5;

        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            var currentAgentLocality = AgentLocalityHelper.GetCurrentLocality(agent, globe);
            if (currentAgentLocality.Head != agent)
            {
                // Только глава города может начинать строительство.
                return false;
            }

            // Если в городе водятся деньги, то глава города может
            // инициировать строительство нового здания.
            var money = currentAgentLocality.Stats.GetResource(LocalityResource.Money);

            return money > 5; // Условно, столько производства требуется для всех зданий.
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            // Выбор здания для строительства.
            // Происходит по одному алгоритму:
            // Каждое второе здание будет электростанция. Чтобы город не остался без энергии.

            var currentAgentLocality = AgentLocalityHelper.GetCurrentLocality(agent, globe);

            // Выбираем произвольный свободный район города.
            var targetRegion = currentAgentLocality.Regions.FirstOrDefault(region => region.Structures.Count < MAX_REGION_CAPABILITY);

            if (targetRegion == null)
            {
                // В городе нет свободный районов.
                return;
            }

            var isEnergyStructure = targetRegion.Structures.Count % 2 != 0;

            if (isEnergyStructure)
            {
                targetRegion.Structures.Add(LocalityStructureRepository.LumberGenerator);
            }
            else
            {
                var structureRoll = dice.Roll(0, 4);
                ILocalityStructure targetStructure;

                switch (structureRoll)
                {
                    case 0:
                        targetStructure = LocalityStructureRepository.GarmentFactory;
                        break;

                    case 1:
                        targetStructure = LocalityStructureRepository.IronMine;
                        break;

                    case 2:
                        targetStructure = LocalityStructureRepository.LumberGenerator;
                        break;

                    case 3:
                        targetStructure = LocalityStructureRepository.PigFarm;
                        break;

                    case 4:
                        targetStructure = LocalityStructureRepository.LivingSector;
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                targetRegion.Structures.Add(targetStructure);
            }
        }
    }
}
