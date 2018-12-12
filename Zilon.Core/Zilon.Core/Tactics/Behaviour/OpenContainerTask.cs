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
        private readonly IMap _map;

        public OpenContainerTask([NotNull] IActor actor,
            [NotNull] IPropContainer container,
            [NotNull] IOpenContainerMethod method,
            [NotNull] IMap map) : base(actor)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _method = method ?? throw new ArgumentNullException(nameof(method));
            _map = map ?? throw new ArgumentNullException(nameof(map));
        }

        protected override void ExecuteTask()
        {
            var actorHexNode = (HexNode)Actor.Node;
            var containerHexNode = (HexNode)_container.Node;

            var actorCoords = actorHexNode.CubeCoords;
            var containerCoords = containerHexNode.CubeCoords;

            var distance = actorCoords.DistanceTo(containerCoords);
            if (distance > 1)
            {
                throw new InvalidOperationException("Невозможно взаимодействовать с контейнером на расстоянии больше 1.");
            }

            var targetIsOnLine = MapHelper.CheckNodeAvailability(_map, Actor.Node, containerHexNode);

            if (!targetIsOnLine)
            {
                throw new InvalidOperationException("Задачу на открытие сундука нельзя выполнить сквозь стены.");
            }

            Actor.OpenContainer(_container, _method);
        }
    }
}
