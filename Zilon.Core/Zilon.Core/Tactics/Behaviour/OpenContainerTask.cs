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
        private readonly IPropContainer _container;
        private readonly IOpenContainerMethod _method;
        private readonly ISectorMap _map;

        public OpenContainerTask([NotNull] IActor actor,
            [NotNull] IPropContainer container,
            [NotNull] IOpenContainerMethod method,
            [NotNull] ISectorMap map) : base(actor)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _method = method ?? throw new ArgumentNullException(nameof(method));
            _map = map ?? throw new ArgumentNullException(nameof(map));
        }

        protected override void ExecuteTask()
        {
            var distance = _map.DistanceBetween(Actor.Node, _container.Node);
            if (distance > 1)
            {
                throw new InvalidOperationException("Невозможно взаимодействовать с контейнером на расстоянии больше 1.");
            }

            var targetIsOnLine = _map.TargetIsOnLine(Actor.Node, _container.Node);

            if (!targetIsOnLine)
            {
                throw new InvalidOperationException("Задачу на открытие сундука нельзя выполнить сквозь стены.");
            }

            Actor.OpenContainer(_container, _method);
        }
    }
}
