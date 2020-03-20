using Zilon.Core.CommonServices.Dices;
using Zilon.Core.WorldGeneration;
using Zilon.Core.WorldGeneration.AgentCards;

namespace Zilon.Core.World.AgentCards
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
            globe.LocalitiesCells.TryGetValue(agent.Location, out var currentLocality);

            TransportHelper.TransportAgentToRandomLocality(globe, dice, agent, currentLocality);
        }
    }
}
