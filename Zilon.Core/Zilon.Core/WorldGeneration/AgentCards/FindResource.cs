using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    /// <summary>
    /// Агент обеспечивает город ресурсом.
    /// Ресурс выбирается в зависимости от доминирующего навыка агента.
    /// Успешность получения ресурса зависит от уровня навыка.
    /// </summary>
    public sealed class FindResource : IAgentCard
    {
        public int PowerCost => 1;

        public bool CanUse(Agent agent, Globe globe)
        {
            return true;
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            var agentTopSkill = GetTopSkill(agent);
            if (agentTopSkill.Value == 0)
            {
                // Этот агент ничего не умеет.
                // У него нет никаких навыков.
                return;
            }

            var skill = agentTopSkill.Key;
            var skillLevel = agentTopSkill.Value;

            RollResouce(agent, globe, dice, skill, skillLevel);

            RollLevelUp(agent, dice, skill, skillLevel);
        }

        private void RollResouce(Agent agent, Globe globe, IDice dice, BranchType skill, int skillLevel)
        {
            var huntRoll = dice.Roll2D6();
            // Чем выше навык, тем больше шансов, то агент добудет ресурс.
            var successHuntRoll = 7 - (skillLevel - 1);

            if (huntRoll >= successHuntRoll)
            {
                var resource = GetResourceBySkill(skill);
                var currentAgentLocality = AgentLocalityHelper.GetCurrentLocality(agent, globe);
                currentAgentLocality.Stats.AddResource(resource, 1);
            }
        }

        /// <summary>
        /// С некоторой вероятностью агент прокачивает навык, которй использовал.
        /// Успешность развития навыка зависит от текущего навыка.
        /// Нужно выбрость больше, чем удвоенный текущий уровнь навыка.
        /// Таким образом на сборе ресурсов агент может подняться только до 7 уровня навыка.
        /// </summary>
        private static void RollLevelUp(Agent agent, IDice dice, BranchType skill, int skillLevel)
        {
            var skillLevelUpRoll = dice.Roll2D6();
            var successLevelUpRoll = skillLevel * 2;
            if (skillLevelUpRoll >= successLevelUpRoll)
            {
                agent.Skills[skill]++;
            }
        }

        private static KeyValuePair<BranchType, int> GetTopSkill(Agent agent)
        {
            return agent.Skills.OrderByDescending(x => x.Value).FirstOrDefault();
        }

        private LocalityResource GetResourceBySkill(BranchType branch)
        {
            switch (branch)
            {
                case BranchType.Agricultural:
                    return LocalityResource.Food;

                case BranchType.Culture:
                    return LocalityResource.Goods;

                case BranchType.Defense:
                    return LocalityResource.Money;

                case BranchType.Industry:
                    return LocalityResource.Manufacture;

                case BranchType.Politics:
                    return LocalityResource.Money;

                case BranchType.Spirituality:
                    return LocalityResource.Food;

                case BranchType.Tourism:
                    return LocalityResource.Goods;

                case BranchType.Trade:
                    return LocalityResource.Manufacture;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
