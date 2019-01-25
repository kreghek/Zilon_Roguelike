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

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            var realmLocalities = globe.localities.Where(x => x.Owner == agent.Realm).ToArray();
            var rolledTransportLocalityIndex = dice.Roll(0, realmLocalities.Length - 1);
            var rolledTransportLocality = realmLocalities[rolledTransportLocalityIndex];

            Helper.RemoveAgentToCell(globe.agentCells, agent.Localtion, agent);

            agent.Localtion = rolledTransportLocality.Cells[0];

            Helper.AddAgentToCell(globe.agentCells, agent.Localtion, agent);
        }
    }
}
