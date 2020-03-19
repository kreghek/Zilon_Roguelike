using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Props;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class ActIsNotAvailableTrigger : ILogicStateTrigger
    {
        private readonly ISectorUiState _sectorUiState;

        public ActIsNotAvailableTrigger(ISectorUiState sectorUiState)
        {
            _sectorUiState = sectorUiState;
        }

        public void Reset()
        {
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStrategyData strategyData)
        {
            var currentAct = _sectorUiState.TacticalAct;

            // Проверка наличия ресурсов для выполнения действия

            if (currentAct.Constrains?.PropResourceType != null && currentAct.Constrains?.PropResourceCount != null)
            {
                var hasPropResource = CheckPropResource(_sectorUiState.ActiveActor.Actor.Person.Inventory,
                    currentAct.Constrains.PropResourceType,
                    currentAct.Constrains.PropResourceCount.Value);

                if (!hasPropResource)
                {
                    return true;
                }
            }

            // Проверка КД

            if (currentAct.CurrentCooldown > 0)
            {
                return true;
            }

            return false;
        }

        public void Update()
        {
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
