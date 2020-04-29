using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Graphs;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Базовая реализация сервиса для работы с действиями персонажа.
    /// </summary>
    /// <seealso cref="ITacticalActUsageService" />
    public sealed class TacticalActUsageService : ITacticalActUsageService
    {
        private readonly ITacticalActUsageRandomSource _actUsageRandomSource;
        private readonly ISectorManager _sectorManager;
        private readonly IActUsageHandlerSelector _actUsageHandlerSelector;

        /// <summary>Сервис для работы с прочностью экипировки.</summary>
        public IEquipmentDurableService EquipmentDurableService { get; set; }

        /// <summary>
        /// Конструирует экземпляр службы <see cref="TacticalActUsageService"/>.
        /// </summary>
        /// <param name="actUsageRandomSource">Источник рандома для выполнения действий.</param>
        /// <param name="perkResolver">Сервис для работы с прогрессом перков.</param>
        /// <param name="sectorManager">Менеджер сектора.</param>
        /// <exception cref="System.ArgumentNullException">
        /// actUsageRandomSource
        /// or
        /// perkResolver
        /// or
        /// sectorManager
        /// </exception>
        public TacticalActUsageService(
            ITacticalActUsageRandomSource actUsageRandomSource,
            ISectorManager sectorManager,
            IActUsageHandlerSelector actUsageHandlerSelector)
        {
            _actUsageRandomSource = actUsageRandomSource ?? throw new ArgumentNullException(nameof(actUsageRandomSource));
            _sectorManager = sectorManager ?? throw new ArgumentNullException(nameof(sectorManager));
            _actUsageHandlerSelector = actUsageHandlerSelector ?? throw new ArgumentNullException(nameof(actUsageHandlerSelector));
        }

        public TacticalActUsageService(
            ITacticalActUsageRandomSource actUsageRandomSource,
            ISectorManager sectorManager,
            IActUsageHandlerSelector actUsageHandlerSelector,
            IEquipmentDurableService equipmentDurableService) : this(actUsageRandomSource, sectorManager, actUsageHandlerSelector)
        {
            EquipmentDurableService = equipmentDurableService ?? throw new ArgumentNullException(nameof(equipmentDurableService));
        }

        public void UseOn(IActor actor, IAttackTarget target, UsedTacticalActs usedActs)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (usedActs is null)
            {
                throw new ArgumentNullException(nameof(usedActs));
            }

            foreach (var act in usedActs.Primary)
            {
                if (!act.Stats.Targets.HasFlag(TacticalActTargets.Self) && actor == target)
                {
                    throw new ArgumentException("Актёр не может атаковать сам себя", nameof(target));
                }

                UseAct(actor, target, act);
            }

            // Использование дополнительных действий.
            // Используются с некоторой вероятностью.
            foreach (var act in usedActs.Secondary)
            {
                var useSuccessRoll = GetUseSuccessRoll();
                var useFactRoll = GetUseFactRoll();

                if (useFactRoll < useSuccessRoll)
                {
                    continue;
                }

                UseAct(actor, target, act);
            }
        }

        private static IEnumerable<IGraphNode> GetActorNodes(PhysicalSize physicalSize, IGraphNode baseNode, IMap map)
        {
            yield return baseNode;

            if (physicalSize == PhysicalSize.Size7)
            {
                var neighbors = map.GetNext(baseNode);
                foreach (var neighbor in neighbors)
                {
                    yield return neighbor;
                }
            }
        }

        private void UseAct(IActor actor, IAttackTarget target, ITacticalAct act)
        {
            bool isInDistance;
            if ((act.Stats.Targets & TacticalActTargets.Self) > 0 && actor == target)
            {
                isInDistance = true;
            }
            else
            {
                isInDistance = IsInDistance(actor, target, act);
            }

            if (!isInDistance)
            {
                throw new UsageOutOfDistanceException("Попытка атаковать цель, находящуюся за пределами атаки.");
            }

            var targetNode = target.Node;

            var targetIsOnLine = _sectorManager.CurrentSector.Map.TargetIsOnLine(
                actor.Node,
                targetNode);

            if (!targetIsOnLine)
            {
                throw new UsageThroughtWallException("Задачу на атаку нельзя выполнить сквозь стены.");
            }

            actor.UseAct(target, act);

            var tacticalActRoll = GetActEfficient(act);

            // Изъятие патронов
            if (act.Constrains?.PropResourceType != null)
            {
                RemovePropResource(actor, act);
            }

            var actHandler = _actUsageHandlerSelector.GetHandler(target);
            actHandler.ProcessActUsage(actor, target, tacticalActRoll);

            if (act.Equipment != null)
            {
                EquipmentDurableService?.UpdateByUse(act.Equipment, actor.Person);
            }

            // Сброс КД, если он есть.
            act.StartCooldownIfItIs();
        }

        private bool IsInDistance(IActor actor, IAttackTarget target, ITacticalAct act)
        {
            var actorNodes = GetActorNodes(actor.PhysicalSize, actor.Node, _sectorManager.CurrentSector.Map);
            var targetNodes = GetActorNodes(target.PhysicalSize, target.Node, _sectorManager.CurrentSector.Map);
            foreach (var node in actorNodes)
            {
                foreach (var targetNode in targetNodes)
                {
                    var isInDistanceInNode = act.CheckDistance(node, targetNode, _sectorManager.CurrentSector.Map);
                    if (isInDistanceInNode)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void RemovePropResource(IActor actor, ITacticalAct act)
        {
            var propResources = from prop in actor.Person.GetModule<IInventoryModule>().CalcActualItems()
                                where prop is Resource
                                where prop.Scheme.Bullet?.Caliber == act.Constrains.PropResourceType
                                select prop;

            if (propResources.FirstOrDefault() is Resource propResource)
            {
                if (propResource.Count >= act.Constrains.PropResourceCount)
                {
                    var usedResource = new Resource(propResource.Scheme, act.Constrains.PropResourceCount.Value);
                    actor.Person.GetModule<IInventoryModule>().Remove(usedResource);
                }
                else
                {
                    throw new InvalidOperationException($"Не хватает ресурса {propResource} для использования действия {act}.");
                }
            }
            else
            {
                throw new InvalidOperationException($"Не хватает ресурса {act.Constrains?.PropResourceType} для использования действия {act}.");
            }
        }

        private int GetUseFactRoll()
        {
            var roll = _actUsageRandomSource.RollUseSecondaryAct();
            return roll;
        }

        private int GetUseSuccessRoll()
        {
            // В будущем успех использования вторичных дейсвий будет зависить от действия, экипировки, перков.
            return 5;
        }

        /// <summary>
        /// Возвращает случайное значение эффективность действия.
        /// </summary>
        /// <param name="act"> Соверщённое действие. </param>
        /// <returns> Возвращает выпавшее значение эффективности. </returns>
        private TacticalActRoll GetActEfficient(ITacticalAct act)
        {
            var rolledEfficient = _actUsageRandomSource.RollEfficient(act.Efficient);

            var roll = new TacticalActRoll(act, rolledEfficient);

            return roll;
        }
    }
}
