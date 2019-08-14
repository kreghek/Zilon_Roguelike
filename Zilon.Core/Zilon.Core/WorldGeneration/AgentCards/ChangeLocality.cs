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
            globe.LocalitiesCells.TryGetValue(agent.Location, out var currentLocality);

            TransportHelper.TransportAgentToRandomLocality(globe, dice, agent, currentLocality);
        }
    }
}
