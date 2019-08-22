using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.AgentCards
{
    public sealed class Hunt : IAgentCard
    {
        public int PowerCost => 1;

        public bool CanUse(Agent agent, Globe globe)
        {
            return true;
        }

        public void Use(Agent agent, Globe globe, IDice dice)
        {
            var huntRoll = dice.Roll2D6();
            var successHuntRoll = 7;

            if (huntRoll >= successHuntRoll)
            {
                var currentAgentLocality = AgentLocalityHelper.GetCurrentLocality(agent, globe);
                currentAgentLocality.Stats.AddResource(LocalityResource.Food, 1);
            }
        }
    }
}
