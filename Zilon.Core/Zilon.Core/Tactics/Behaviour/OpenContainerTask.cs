﻿using System;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Задача актёра на подбор предмета из сектора (с земли или из контейнера) в инвентарь
    /// </summary>
    public class OpenContainerTask : OneTurnActorTaskBase
    {
        private readonly IOpenContainerMethod _method;
        private readonly IStaticObject _staticObject;

        public OpenContainerTask(IActor actor,
            IActorTaskContext context,
            IStaticObject staticObject,
            IOpenContainerMethod method) : base(actor, context)
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
                throw new InvalidOperationException(
                    "Невозможно взаимодействовать с контейнером на расстоянии больше 1.");
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