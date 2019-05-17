using System.Linq;

using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public sealed class LootLogicState : ILogicState
    {
        private readonly IPropContainerManager _propContainerManager;
        private readonly ISectorMap _map;

        public LootLogicState(IPropContainerManager propContainerManager, ISectorManager sectorManager)
        {
            _propContainerManager = propContainerManager;
            _map = sectorManager.CurrentSector.Map;
        }

        public bool Complete { get; private set; }

        public ILogicStateData CreateData(IActor actor)
        {
            var foundContainers = LootHelper.FindAvailableContainers(_propContainerManager.Items,
                actor.Node,
                _map);

            var orderedContainers = foundContainers.OrderBy(x => _map.DistanceBetween(actor.Node, x.Node));
            var nearbyContainer = orderedContainers.FirstOrDefault();

            var data = new LootLogicData(nearbyContainer);

            return data;
        }

        public IActorTask GetTask(IActor actor, ILogicStateData data)
        {
            var logicData = (LootLogicData)data;
            if (logicData.PropContainer == null)
            {
                Complete = true;
                return null;
            }

            var distance = _map.DistanceBetween(actor.Node, logicData.PropContainer.Node);
            if (distance <= 1)
            {
                return TakeAllFromContainerTask(actor, logicData.PropContainer);
            }
            else
            {
                var storedMoveTask = logicData.MoveTask;
                var moveTask = MoveToContainerTask(actor, logicData.PropContainer.Node, storedMoveTask);
                logicData.MoveTask = moveTask;
                return moveTask;
            }
        }

        private MoveTask MoveToContainerTask(IActor actor, IMapNode containerMapNode, MoveTask storedMoveTask)
        {
            var moveTask = storedMoveTask;
            if (storedMoveTask == null)
            {
                moveTask = new MoveTask(actor, containerMapNode, _map);
            }

            if (storedMoveTask.IsComplete || !storedMoveTask.CanExecute())
            {
                Complete = true;
                return null;
            }

            return moveTask;
        }

        private static IActorTask TakeAllFromContainerTask(IActor actor, IPropContainer container)
        {
            var inventoryTransfer = new PropTransfer(actor.Person.Inventory,
                                container.Content.CalcActualItems(),
                                new IProp[0]);

            var containerTransfer = new PropTransfer(container.Content,
                new IProp[0],
                container.Content.CalcActualItems());

            return new TransferPropsTask(actor, new[] { inventoryTransfer, containerTransfer });
        }
    }
}
