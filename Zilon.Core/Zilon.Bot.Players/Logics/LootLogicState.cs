using Zilon.Core.Graphs;
using Zilon.Core.PersonModules;
using Zilon.Core.Props;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public sealed class LootLogicState : LogicStateBase
    {
        private MoveTask _moveTask;

        public static IStaticObject FindContainer(
            IActor actor,
            IStaticObjectManager staticObjectManager,
            ISectorMap map)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            if (staticObjectManager is null)
            {
                throw new System.ArgumentNullException(nameof(staticObjectManager));
            }

            if (map is null)
            {
                throw new System.ArgumentNullException(nameof(map));
            }

            var containerStaticObjects = staticObjectManager.Items
                .Where(x => x.HasModule<IPropContainer>());

            var foundContainers = LootHelper.FindAvailableContainers(containerStaticObjects,
                actor.Node,
                map);

            var orderedContainers = foundContainers.OrderBy(x => map.DistanceBetween(actor.Node, x.Node));
            var nearbyContainer = orderedContainers.FirstOrDefault();

            return nearbyContainer;
        }

        public override IActorTask GetTask(
            IActor actor,
            ISectorTaskSourceContext context,
            ILogicStrategyData strategyData)
        {
            var map = context.Sector.Map;
            var staticObjectManager = context.Sector.StaticObjectManager;
            var staticObject = FindContainer(actor, staticObjectManager, map);

            if ((staticObject == null) || !staticObject.GetModule<IPropContainer>().Content.CalcActualItems().Any())
            {
                Complete = true;
                return null;
            }

            var distance = map.DistanceBetween(actor.Node, staticObject.Node);
            if (distance <= 1)
            {
                return TakeAllFromContainerTask(actor, staticObject, context.Sector);
            }

            var storedMoveTask = _moveTask;
            var moveTask = MoveToContainerTask(actor, staticObject.Node, storedMoveTask, context.Sector);
            _moveTask = moveTask;
            return moveTask;
        }

        protected override void ResetData()
        {
            _moveTask = null;
        }

        private MoveTask MoveToContainerTask(
            IActor actor,
            IGraphNode containerMapNode,
            MoveTask storedMoveTask,
            ISector sector)
        {
            var map = sector.Map;

            var moveTask = storedMoveTask;
            if (storedMoveTask == null)
            {
                var taskContext = new ActorTaskContext(sector);
                moveTask = new MoveTask(actor, taskContext, containerMapNode, map);
            }

            if (moveTask.IsComplete || !moveTask.CanExecute())
            {
                Complete = true;
                return null;
            }

            return moveTask;
        }

        private static IActorTask TakeAllFromContainerTask(IActor actor, IStaticObject container, ISector sector)
        {
            var inventoryTransfer = new PropTransfer(actor.Person.GetModule<IInventoryModule>(),
                container.GetModule<IPropContainer>().Content.CalcActualItems(),
                System.Array.Empty<IProp>());

            var containerTransfer = new PropTransfer(container.GetModule<IPropContainer>().Content,
                System.Array.Empty<IProp>(),
                container.GetModule<IPropContainer>().Content.CalcActualItems());

            var taskContext = new ActorTaskContext(sector);
            return new TransferPropsTask(actor, taskContext, new[]
            {
                inventoryTransfer, containerTransfer
            });
        }
    }
}