using Zilon.Core.CommonServices.Dices;
using Zilon.Core.World;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Карта действия деятеля.
    /// </summary>
    public interface IAgentCard
    {
        bool CanUse(Agent agent, Globe globe);
        void Use(Agent agent, Globe globe, IDice dice);
        int PowerCost { get; }
    }
}
