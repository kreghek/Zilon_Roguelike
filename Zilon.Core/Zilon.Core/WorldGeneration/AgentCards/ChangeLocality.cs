using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    public class ChangeLocality : IAgentCard
    {
        public int PowerCost { get; }

        public bool CanUse(Agent agent, Globe globe)
        {
            // дальше будет проверка, что деятель не может менять местоположение, пока он занимает пост (например, является императором)
            return true;
        }

        public string Use(Agent agent, Globe globe, IDice dice)
        {
            string history = null;

            globe.LocalitiesCells.TryGetValue(agent.Location, out var currentLocality);

            var rolledTransportLocality = TransportHelper.RollTargetLocality(globe, dice, agent, currentLocality);
            if (rolledTransportLocality == null)
            {
                return history;
            }

            if (currentLocality != null)
            {
                Helper.RemoveAgentFromCell(globe.AgentCells, agent.Location, agent);
            }

            agent.Location = rolledTransportLocality.Cell;

            Helper.AddAgentToCell(globe.AgentCells, agent.Location, agent);

            return $"{agent} was change current location. Now he is in {agent.Location}.";
        }
    }
}
