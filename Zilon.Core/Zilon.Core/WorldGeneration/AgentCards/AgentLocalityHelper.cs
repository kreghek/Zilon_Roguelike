namespace Zilon.Core.WorldGeneration.AgentCards
{
    public static class AgentLocalityHelper
    {
        public static Locality GetCurrentLocality(Agent agent, Globe globe)
        {
            var currentAgentCell = agent.Location;
            var currentAgentLocality = globe.LocalitiesCells[currentAgentCell];
            return currentAgentLocality;
        }
    }
}
