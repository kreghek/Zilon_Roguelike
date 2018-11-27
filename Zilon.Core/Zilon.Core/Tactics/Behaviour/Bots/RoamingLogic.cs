using System;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    /// <summary>
    /// Логика произвольного брожения.
    /// </summary>
    public class RoamingLogic : AgressiveLogicBase
    {
        public RoamingLogic(IActor actor,
            IMap map,
            IActorManager actors,
            IDecisionSource decisionSource,
            ITacticalActUsageService actService): base(actor, map, actors, decisionSource, actService)
        {

        }

        protected override void ProcessMovementComplete()
        {
            // Пока никак не обрабатываем.
            // В конечном счёте сдесь можно сделать учёт уже пройденных узлов комнаты.
        }

        protected override MoveTask CreateBypassMoveTask()
        {
            //_roamingTask = new IdleTask(_actor, _decisionSource);

            //return _roamingTask;
            throw new NotImplementedException();
        }

        protected override void ProcessIntruderDetected()
        {
            // Ничего делать не нужно.
        }
    }
}
