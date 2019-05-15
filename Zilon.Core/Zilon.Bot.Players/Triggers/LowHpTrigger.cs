using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Triggers
{
    public class LowHpTrigger : ILogicStateTrigger
    {
        public ILogicStateData CreateData(IActor actor)
        {
            return new EmptyLogicTriggerData();
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStateData data)
        {
            var hpStat = actor.Person.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health);
            var hpStatCoeff = (float)hpStat.Value / (hpStat.Range.Max - hpStat.Range.Min);
            var isLowHp = hpStatCoeff <= 0.1f;
            return isLowHp;
        }
    }
}
