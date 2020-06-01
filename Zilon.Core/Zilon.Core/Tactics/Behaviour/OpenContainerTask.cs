using System;

using JetBrains.Annotations;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Задача актёра на подбор предмета из сектора (с земли или из контейнера) в инвентарь
    /// </summary>
    public class OpenContainerTask : OneTurnActorTaskBase
    {
        private readonly IStaticObject _staticObject;
        private readonly IOpenContainerMethod _method;

        public OpenContainerTask([NotNull] IActor actor,
            [NotNull] IActorTaskContext context,
            [NotNull] IStaticObject staticObject,
            [NotNull] IOpenContainerMethod method) : base(actor, context)
        {
            _staticObject = staticObject ?? throw new ArgumentNullException(nameof(staticObject));
            _method = method ?? throw new ArgumentNullException(nameof(method));
        }

        protected override void ExecuteTask()
        {
            var map = Context.Sector.Map;
            var distance = map.DistanceBetween(Actor.Node, _staticObject.Node);
            if (distance > 1)
            {
                throw new InvalidOperationException("Невозможно взаимодействовать с контейнером на расстоянии больше 1.");
            }

            var targetIsOnLine = map.TargetIsOnLine(Actor.Node, _staticObject.Node);

            if (!targetIsOnLine)
            {
                throw new InvalidOperationException("Задачу на открытие сундука нельзя выполнить сквозь стены.");
            }

            Actor.OpenContainer(_staticObject, _method);
        }
    }
}
