using System;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public abstract class MoveLogicStateBase : LogicStateBase
    {
        protected MoveTask MoveTask { get; set; }
        protected IdleTask IdleTask { get; set; }

        protected IDecisionSource DecisionSource { get; }

        protected ISectorMap Map { get; }

        protected MoveLogicStateBase(IDecisionSource decisionSource, ISectorManager sectorManager)
        {
            if (sectorManager is null)
            {
                throw new ArgumentNullException(nameof(sectorManager));
            }

            DecisionSource = decisionSource ?? throw new ArgumentNullException(nameof(decisionSource));
            Map = sectorManager.CurrentSector.Map;
        }

        protected override void ResetData()
        {
            MoveTask = null;
            IdleTask = null;
        }
    }
}