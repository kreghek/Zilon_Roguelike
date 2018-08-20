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
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _method = method ?? throw new ArgumentNullException(nameof(method));
        }

        protected override void ExecuteTask()
        {
            //TODO Сделать проверку, чтобы нельзя было открывать сундуки через стены аналогично команде на открытие.
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
