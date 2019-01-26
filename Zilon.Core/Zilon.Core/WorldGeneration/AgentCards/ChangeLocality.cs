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
            globe.LocalitiesCells.TryGetValue(agent.Localtion, out var currentLocality);

            var realmLocalities = globe.Localities.Where(x => x.Owner == agent.Realm && currentLocality != x).ToArray();
            if (!realmLocalities.Any())
            {
                return;
            }

            var rolledTransportLocalityIndex = dice.Roll(0, realmLocalities.Length - 1);
            var rolledTransportLocality = realmLocalities[rolledTransportLocalityIndex];

            if (currentLocality != null)
            {
                Helper.RemoveAgentToCell(globe.AgentCells, agent.Localtion, agent);
            }

            agent.Localtion = rolledTransportLocality.Cell;

            Helper.AddAgentToCell(globe.AgentCells, agent.Localtion, agent);
        }
    }
}
