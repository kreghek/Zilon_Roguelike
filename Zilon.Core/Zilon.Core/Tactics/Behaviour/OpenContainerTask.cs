using System;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Задача актёра на подбор предмета из сектора (с земли или из контейнера) в инвентарь
    /// </summary>
    public class OpenContainerTask : ActorTaskBase
    {
        private readonly IPropContainer _container;
        private readonly IOpenContainerMethod _method;

        protected OpenContainerTask(IActor actor, IPropContainer container, IOpenContainerMethod method) : base(actor)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            _container = container;
            _method = method;
        }

        public override void Execute()
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

            var openResult = _method.TryOpen(_container);

            _container.Open();

            IsComplete = true;
        }
    }
}
