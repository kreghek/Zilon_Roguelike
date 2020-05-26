using System;

using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Bot.Players.Logics
{
    public abstract class MoveLogicStateBase: LogicStateBase
    {
        protected MoveTask MoveTask { get; set; }
        protected IdleTask IdleTask { get; set; }

        protected IDecisionSource DecisionSource { get; }

        protected MoveLogicStateBase(IDecisionSource decisionSource)
        {
            DecisionSource = decisionSource ?? throw new ArgumentNullException(nameof(decisionSource));
        }

        protected override void ResetData()
        {
            MoveTask = null;
            IdleTask = null;
        }
    }
}
