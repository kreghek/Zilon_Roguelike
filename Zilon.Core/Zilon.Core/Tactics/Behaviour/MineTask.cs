using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class MineTask : OneTurnActorTaskBase
    {
        private readonly IMineDepositMethod _method;
        private readonly IStaticObject _staticObject;

        public MineTask([NotNull] IActor actor,
            [NotNull] IActorTaskContext context,
            [NotNull] IStaticObject staticObject,
            [NotNull] IMineDepositMethod method) : base(actor, context)
        {
            _staticObject = staticObject ?? throw new ArgumentNullException(nameof(staticObject));
            _method = method ?? throw new ArgumentNullException(nameof(method));
        }

        protected override void ExecuteTask()
        {
            ISectorMap map = Context.Sector.Map;
            var distance = map.DistanceBetween(Actor.Node, _staticObject.Node);
            if (distance > 1)
            {
                throw new InvalidOperationException("Невозможно взаимодействовать с объектом на расстоянии больше 1.");
            }

            var targetIsOnLine = map.TargetIsOnLine(Actor.Node, _staticObject.Node);

            if (!targetIsOnLine)
            {
                throw new InvalidOperationException("Задачу на добычу нельзя выполнить сквозь стены.");
            }

            Actor.MineDeposit(_staticObject, _method);
        }
    }
}