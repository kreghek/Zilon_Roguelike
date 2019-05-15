using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public sealed class RoamingLogicData: ILogicStateData
    {
        public MoveTask MoveTask { get; set; }

        public IdleTask IdleTask { get; set; }
    }
}
