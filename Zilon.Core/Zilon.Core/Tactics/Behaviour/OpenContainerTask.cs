using System;

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

        public OpenContainerTask(IActor actor, IPropContainer container, IOpenContainerMethod method) : base(actor)
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

        protected override void ExecuteTask()
        {
            //TODO Сделать проверку, чтобы нельзя было открывать сундуки через стены.
            var actorHexNode = (HexNode)Actor.Node;
            var containerHexNode = (HexNode)_container.Node;

            var actorCoords = actorHexNode.CubeCoords;
            var containerCoords = containerHexNode.CubeCoords;

            var distance = actorCoords.DistanceTo(containerCoords);

            if (distance > 1)
            {
                throw new InvalidOperationException("Невозможно взаимодействовать с контейнером на расстоянии больше 1.");
            }

            Actor.OpenContainer(_container, _method);
        }
    }
}
