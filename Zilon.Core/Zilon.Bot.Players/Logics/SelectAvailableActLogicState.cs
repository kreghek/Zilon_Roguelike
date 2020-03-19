using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Client;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public sealed class SelectAvailableActLogicState : LogicStateBase
    {
        private readonly ISectorUiState _sectorUiState;

        public SelectAvailableActLogicState(ISectorUiState sectorUiState)
        {
            _sectorUiState = sectorUiState;
        }

        public override IActorTask GetTask(IActor actor, ILogicStrategyData strategyData)
        {
            var availableActs = actor.Person.TacticalActCarrier.Acts
                .Where(x => x.CurrentCooldown == null || x.CurrentCooldown == 0)
                .Where(x =>
                (x.Constrains.PropResourceType != null && CheckPropResource(actor.Person.Inventory, x.Constrains.PropResourceType, x.Constrains.PropResourceCount.Value)) ||
                (x.Constrains.PropResourceType is null));

            _sectorUiState.TacticalAct = availableActs.FirstOrDefault();

            Complete = true;
            return null;
        }

        protected override void ResetData()
        {
            // Нет состояния.
        }

        private bool CheckPropResource(IPropStore inventory,
            string usedPropResourceType,
            int usedPropResourceCount)
        {
            var props = inventory.CalcActualItems();
            var propResources = new List<Resource>();
            foreach (var prop in props)
            {
                var propResource = prop as Resource;
                if (propResource == null)
                {
                    continue;
                }

                if (propResource.Scheme.Bullet?.Caliber == usedPropResourceType)
                {
                    propResources.Add(propResource);
                }
            }

            var preferredPropResource = propResources.FirstOrDefault();

            return preferredPropResource != null && preferredPropResource.Count >= usedPropResourceCount;
        }
    }
}
