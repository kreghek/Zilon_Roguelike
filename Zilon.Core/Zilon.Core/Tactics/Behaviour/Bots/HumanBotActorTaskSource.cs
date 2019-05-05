using Zilon.Core.Players;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    public class HumanBotActorTaskSource : MonsterActorTaskSource
    {
        public HumanBotActorTaskSource(HumanPlayer player,
            IDecisionSource decisionSource,
            ITacticalActUsageService actService,
            ISectorManager sectorManager,
            IActorManager actorManager): base(player, decisionSource, actService, sectorManager, actorManager)
        {
           
        }

    }
}
