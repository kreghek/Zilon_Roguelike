using System;
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

            var distance = _map.DistanceBetween(actor.Node, logicData.PropContainer.Node);
            if (distance <= 1)
            {
                var transfer = new PropTransfer(logicData.PropContainer.Content,
                    logicData.PropContainer.Content.CalcActualItems(),
                    new IProp[0]);

                var transfer = new PropTransfer(logicData.PropContainer.Content,
                    logicData.PropContainer.Content.CalcActualItems(),
                    new IProp[0]);

                return new TransferPropsTask(actor, new[] { transfer });
            }
        }
    }
}
