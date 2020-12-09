using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IActUsageHandlerSelector _actUsageHandlerSelector;
        private readonly ITacticalActUsageRandomSource _actUsageRandomSource;

        /// <summary>
        /// Конструирует экземпляр службы <see cref="TacticalActUsageService" />.
        /// </summary>
        /// <param name="actUsageRandomSource">Источник рандома для выполнения действий.</param>
        /// <param name="perkResolver">Сервис для работы с прогрессом перков.</param>
        /// <exception cref="System.ArgumentNullException">
        /// actUsageRandomSource
        /// or
        /// perkResolver
        /// or
        /// sectorManager
        /// </exception>
        public TacticalActUsageService(
            ITacticalActUsageRandomSource actUsageRandomSource,
            IActUsageHandlerSelector actUsageHandlerSelector)
        {
            _actUsageRandomSource =
                actUsageRandomSource ?? throw new ArgumentNullException(nameof(actUsageRandomSource));
            _actUsageHandlerSelector = actUsageHandlerSelector ??
                                       throw new ArgumentNullException(nameof(actUsageHandlerSelector));
        }

        public TacticalActUsageService(
            ITacticalActUsageRandomSource actUsageRandomSource,
            IActUsageHandlerSelector actUsageHandlerSelector,
            IEquipmentDurableService equipmentDurableService) : this(actUsageRandomSource, actUsageHandlerSelector)
        {
            EquipmentDurableService = equipmentDurableService ??
                                      throw new ArgumentNullException(nameof(equipmentDurableService));
        }

        /// <summary>Сервис для работы с прочностью экипировки.</summary>
        public IEquipmentDurableService EquipmentDurableService { get; set; }

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

        private static IEnumerable<IGraphNode> GetActorNodes(
            PhysicalSizePattern physicalSize,
            IGraphNode baseNode,
            IMap map)
        {
            yield return baseNode;

            if (physicalSize == PhysicalSizePattern.Size7)
            {
                var neighbors = map.GetNext(baseNode);
                foreach (var neighbor in neighbors)
                {
                    yield return neighbor;
                }
            }
        }

        private int GetUseFactRoll()
        {
            var roll = _actUsageRandomSource.RollUseSecondaryAct();
            return roll;
        }

        private static int GetUseSuccessRoll()
        {
            // В будущем успех использования вторичных дейсвий будет зависить от действия, экипировки, перков.
            return 5;
        }

        private static bool IsInDistance(IActor actor, IGraphNode targetNode, ITacticalAct act, ISectorMap map)
        {
            var actorNodes = GetActorNodes(actor.PhysicalSize, actor.Node, map);

            foreach (var node in actorNodes)
            {
                var isInDistanceInNode = act.CheckDistance(node, targetNode, map);
                if (isInDistanceInNode)
                {
                    return true;
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
                    throw new InvalidOperationException(
                        $"Не хватает ресурса {propResource} для использования действия {act}.");
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"Не хватает ресурса {act.Constrains?.PropResourceType} для использования действия {act}.");
            }
        }

        private static bool SectorHasAttackTarget(ISector sector, IAttackTarget target)
        {
            switch (target)
            {
                case IActor actor:
                    return SectorHasAttackedActor(sector, actor);

                case IStaticObject staticObject:
                    return SectorHasAttackedStaticObject(sector, staticObject);

                default:
                    throw new InvalidOperationException($"Unknown attack target type {target.GetType().FullName}.");
            }
        }

        private static bool SectorHasAttackedActor(ISector sector, IAttackTarget target)
        {
            if (sector.ActorManager is null)
            {
                // In test environment not all sector mocks has actor manager
                return true;
            }

            return sector.ActorManager.Items.Any(x => ReferenceEquals(x, target));
        }

        private static bool SectorHasAttackedStaticObject(ISector sector, IAttackTarget target)
        {
            if (sector.StaticObjectManager is null)
            {
                // In test environment not all sector mocks has actor manager
                return true;
            }

            return sector.StaticObjectManager.Items.Any(x => ReferenceEquals(x, target));
        }

        private static bool SectorHasCurrentActor(ISector sector, IActor actor)
        {
            if (sector.ActorManager is null)
            {
                // In test environment not all sector mocks has actor manager
                return true;
            }

            return sector.ActorManager.Items.Any(x => x == actor);
        }

        private void UseAct(IActor actor, ActTargetInfo target, ITacticalAct act, ISectorMap map)
        {
            bool isInDistance;
            if ((act.Stats.Targets & TacticalActTargets.Self) > 0 && actor == target)
            {
                isInDistance = true;
            }
            else
            {
                isInDistance = IsInDistance(actor, target.TargetNode, act, map);
            }

            if (!isInDistance)
            {
                // Это может произойти, если цель, в процессе применения действия
                // успела выйти из радиуса применения.
                // TODO Лучше сделать, чтобы этот метод возвращал объект с результатом выполнения действия.
                // А внешний код пусть либо считает исход допустимым, либо выбрасывает исключения типа UsageOutOfDistanceException
                return;
            }

            var targetNode = target.TargetNode;

            var targetIsOnLine = map.TargetIsOnLine(
                actor.Node,
                targetNode);

            if (targetIsOnLine)
            {
                actor.UseAct(targetNode, act);

                var tacticalActRoll = GetActEfficient(act);

                var actHandler = _actUsageHandlerSelector.GetHandler(target.TargetObject);
                actHandler.ProcessActUsage(actor, target.TargetObject, tacticalActRoll);

                UseActResources(actor, act);
            }
            else
            {
                // Ситация, когда цель за стеной может произойти по следующим причинам:
                // 1. В момент начала применения действия цель была доступна. К моменту выполнения дейтвия цель скрылась.
                // В этом случае изымаем патроны и начинаем КД по действию, так как фактически ресурсы на него потрачены. Но на цель не воздействуем.
                // 2. Ошибка во внешнем коде, когда не провели предварительную проверку. Это проверяется сравнением ячейки в которую целились
                // на момент начала действия и текущую ячейку цели.

                if (target.TargetNode == target.TargetObject.Node)
                {
                    throw new UsageThroughtWallException();
                }

                UseActResources(actor, act);
            }
        }

        private void UseActResources(IActor actor, ITacticalAct act)
        {
            // Изъятие патронов
            if (act.Constrains?.PropResourceType != null)
            {
                RemovePropResource(actor, act);
            }

            if (act.Equipment != null)
            {
                EquipmentDurableService?.UpdateByUse(act.Equipment, actor.Person);
            }

            // Сброс КД, если он есть.
            act.StartCooldownIfItIs();
        }

        public void UseOn(IActor actor, ActTargetInfo target, UsedTacticalActs usedActs, ISector sector)
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

            if (sector is null)
            {
                throw new ArgumentNullException(nameof(sector));
            }

            Debug.Assert(SectorHasCurrentActor(sector, actor), "Current actor must be in sector");
            Debug.Assert(SectorHasAttackTarget(sector, target.TargetObject), "Target must be in sector");

            foreach (var act in usedActs.Primary)
            {
                if (!act.Stats.Targets.HasFlag(TacticalActTargets.Self) && actor == target)
                {
                    throw new ArgumentException("Актёр не может атаковать сам себя", nameof(target));
                }

                UseAct(actor, target, act, sector.Map);
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

                UseAct(actor, target, act, sector.Map);
            }
        }
    }
}