using System.Linq;

using Zilon.Core.Graphs;
using Zilon.Core.Props;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public sealed class LootLogicState : LogicStateBase
    {
        private IStaticObject _staticObject;

        private MoveTask _moveTask;

        private readonly ISectorMap _map;
        private readonly ISectorManager _sectorManager;

        public LootLogicState(ISectorManager sectorManager)
        {
            if (sectorManager is null)
            {
                throw new System.ArgumentNullException(nameof(sectorManager));
            }

            _map = sectorManager.CurrentSector.Map;
            _sectorManager = sectorManager;
        }

        public IStaticObject FindContainer(IActor actor)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            var containerStaticObjects = _sectorManager.CurrentSector.StaticObjectManager.Items
                .Where(x => x.HasModule<IPropContainer>());

            var foundContainers = LootHelper.FindAvailableContainers(containerStaticObjects,
                actor.Node,
                _map);

            var orderedContainers = foundContainers.OrderBy(x => _map.DistanceBetween(actor.Node, x.Node));
            var nearbyContainer = orderedContainers.FirstOrDefault();

            return nearbyContainer;
        }

        public override IActorTask GetTask(IActor actor, ILogicStrategyData strategyData)
        {
            _staticObject = FindContainer(actor);

            if (_staticObject == null || !_staticObject.GetModule<IPropContainer>().Content.CalcActualItems().Any())
            {
                Complete = true;
                return null;
            }

            var distance = _map.DistanceBetween(actor.Node, _staticObject.Node);
            if (distance <= 1)
            {
                return TakeAllFromContainerTask(actor, _staticObject);
            }
            else
            {
                var storedMoveTask = _moveTask;
                var moveTask = MoveToContainerTask(actor, _staticObject.Node, storedMoveTask);
                _moveTask = moveTask;
                return moveTask;
            }
        }

        private MoveTask MoveToContainerTask(IActor actor, IGraphNode containerMapNode, MoveTask storedMoveTask)
        {
            var moveTask = storedMoveTask;
            if (storedMoveTask == null)
            {
                moveTask = new MoveTask(actor, containerMapNode, _map);
            }

            if (moveTask.IsComplete || !moveTask.CanExecute())
            {
                Complete = true;
                return null;
            }

            return moveTask;
        }

        private static IActorTask TakeAllFromContainerTask(IActor actor, IStaticObject container)
        {
            var inventoryTransfer = new PropTransfer(actor.Person.Inventory,
                                container.GetModule<IPropContainer>().Content.CalcActualItems(),
                                System.Array.Empty<IProp>());

            var containerTransfer = new PropTransfer(container.GetModule<IPropContainer>().Content,
                System.Array.Empty<IProp>(),
                container.GetModule<IPropContainer>().Content.CalcActualItems());

            return new TransferPropsTask(actor, new[] { inventoryTransfer, containerTransfer });
        }

        protected override void ResetData()
        {
            _staticObject = null;
            _moveTask = null;
        }
    }
}
