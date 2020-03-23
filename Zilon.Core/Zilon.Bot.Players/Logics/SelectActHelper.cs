using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Persons;
using Zilon.Core.Props;

namespace Zilon.Bot.Players.Logics
{
    public static class SelectActHelper
    {
        /// <summary>
        /// Выбирает лучшее действие из указанных.
        /// </summary>
        /// <param name="person"> Персонаж, для которого выбирается действие. </param>
        /// <returns> Лучшее дейсвие среди указанных. </returns>
        [NotNull]
        public static ITacticalAct SelectBestAct(IPerson person)
        {
            if (person is null)
            {
                throw new System.ArgumentNullException(nameof(person));
            }

            var availableActs = person.TacticalActCarrier.Acts
                .Where(x => x.CurrentCooldown == null || x.CurrentCooldown == 0)
                .Where(x => TacticalActIsAvailableByConstrains(x, person))
                .OrderBy(x => x.Efficient.Dice * x.Efficient.Count);

            return availableActs.First();
        }

        private static bool TacticalActIsAvailableByConstrains(ITacticalAct tacticalAct, IPerson person)
        {
            if (tacticalAct.Constrains is null)
            {
                // Если нет никаких ограничений, то действие доступно в любом случае.
                return true;
            }

            if (tacticalAct.Constrains.PropResourceType is null)
            {
                // Если нет ограничений по ресурсам, то действие доступно.
                return true;
            }

            // Проверяем наличие ресурсов в нужном количестве.
            // Проверка осуществляется в хранилище, указанном параметром.

            if (!person.HasInventory)
            {
                // Персонажи бе инвентаря не могут применять действия,
                // для которых нужны ресурсы.
                return false;
            }

            var propResourceType = tacticalAct.Constrains.PropResourceType;
            var propResourceCount = tacticalAct.Constrains.PropResourceCount.Value;
            if (CheckPropResource(person.Inventory, propResourceType, propResourceCount))
            {
                return true;
            }

            return false;
        }

        private static bool CheckPropResource(IPropStore inventory,
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
