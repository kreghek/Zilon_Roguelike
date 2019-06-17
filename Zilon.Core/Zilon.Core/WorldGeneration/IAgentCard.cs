using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Карта действия деятеля.
    /// </summary>
    public interface IAgentCard
    {
        bool CanUse(Agent agent, Globe globe);
        string Use(Agent agent, Globe globe, IDice dice);
        int PowerCost { get; }
    }
}
