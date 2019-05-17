using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public sealed class LootLogicData: ILogicStateData
    {
        public LootLogicData(IPropContainer propContainer)
        {
            PropContainer = propContainer;
        }

        public IPropContainer PropContainer { get; }

        public MoveTask MoveTask { get; set; }
    }
}
