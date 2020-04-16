using System;

using JetBrains.Annotations;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class MineTask : OneTurnActorTaskBase
    {
        private readonly IStaticObject _staticObject;
        private readonly IMineDepositMethod _method;
        private readonly ISectorMap _map;

        public MineTask([NotNull] IActor actor,
            [NotNull] IStaticObject staticObject,
            [NotNull] IMineDepositMethod method,
            [NotNull] ISectorMap map) : base(actor)
        {
            _staticObject = staticObject ?? throw new ArgumentNullException(nameof(staticObject));
            _method = method ?? throw new ArgumentNullException(nameof(method));
            _map = map ?? throw new ArgumentNullException(nameof(map));
        }

        protected override void ExecuteTask()
        {
            var distance = _map.DistanceBetween(Actor.Node, _staticObject.Node);
            if (distance > 1)
            {
                throw new InvalidOperationException("Невозможно взаимодействовать с объектом на расстоянии больше 1.");
            }

            var targetIsOnLine = _map.TargetIsOnLine(Actor.Node, _staticObject.Node);

            if (!targetIsOnLine)
            {
                throw new InvalidOperationException("Задачу на добычу нельзя выполнить сквозь стены.");
            }

            Actor.MineDeposit(_staticObject, _method);
        }
    }
}
