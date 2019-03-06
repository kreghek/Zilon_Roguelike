using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Базовая реализация сектора.
    /// </summary>
    /// <seealso cref="Zilon.Core.Tactics.ISector" />
    public class Sector : ISector
    {
        private readonly IActorManager _actorManager;
        private readonly IPropContainerManager _propContainerManager;
        private readonly ITraderManager _traderManager;
        private readonly IDropResolver _dropResolver;
        private readonly ISchemeService _schemeService;
        private readonly IEquipmentDurableService _equipmentDurableService;

        /// <summary>
        /// Событие выстреливает, когда группа актёров игрока покинула сектор.
        /// </summary>
        public event EventHandler<SectorExitEventArgs> HumanGroupExit;

        /// <summary>
        /// Карта в основе сектора.
        /// </summary>
        public ISectorMap Map { get; }

        /// <summary>
        /// Маршруты патрулирования в секторе.
        /// </summary>
        public Dictionary<IActor, IPatrolRoute> PatrolRoutes { get; }

        /// <summary>
        /// Стартовые узлы.
        /// Набор узлов, где могут располагаться актёры игрока
        /// на начало прохождения сектора.
        /// </summary>
        public IMapNode[] StartNodes { get; set; }

        /// <summary>
        /// Менеджер работы с очками.
        /// </summary>
        public IScoreManager ScoreManager { get; set; }

        public string Sid { get; set; }

        public ILocationScheme Scheme { get; set; }

        [ExcludeFromCodeCoverage]
        public Sector(ISectorMap map,
            IActorManager actorManager,
            IPropContainerManager propContainerManager,
            ITraderManager traderManager,
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IEquipmentDurableService equipmentDurableService)
        {
            _actorManager = actorManager;
            _propContainerManager = propContainerManager;
            _traderManager = traderManager;
            _dropResolver = dropResolver;
            _schemeService = schemeService;
            _equipmentDurableService = equipmentDurableService;

            _actorManager.Added += ActorManager_Added;
            _propContainerManager.Added += PropContainerManager_Added;
            _propContainerManager.Removed += PropContainerManager_Remove;

            Map = map ?? throw new ArgumentException("Не передана карта сектора.", nameof(map));

            PatrolRoutes = new Dictionary<IActor, IPatrolRoute>();
        }

        /// <summary>
        /// Обновление состояния сектора.
        /// </summary>
        /// <remarks>
        /// Выполняет ход игрового сектора.
        /// Собирает текущие задачи для всех актёров в секторе.
        /// Выполняет все задачи для каждого актёра.
        /// </remarks>
        public void Update()
        {
            UpdateSurvivals();

            UpdateActorEffects();

            UpdateEquipments();

            // Определяем, не покинули ли актёры игрока сектор.
            DetectSectorExit();
        }

        private void UpdateActorEffects()
        {
            foreach (var actor in _actorManager.Items)
            {
                var effects = actor.Person.Effects;
                foreach (var effect in effects.Items)
                {
                    if (effect is ISurvivalStatEffect actorEffect)
                    {
                        actorEffect.Apply(actor.Person.Survival);
                    }
                }
            }
        }

        private void UpdateSurvivals()
        {
            var actors = _actorManager.Items.ToArray();
            foreach (var actor in actors)
            {
                var survival = actor.Person.Survival;
                if (survival == null)
                {
                    continue;
                }

                survival.Update();
            }
        }

        private void UpdateEquipments()
        {
            var actors = _actorManager.Items.ToArray();
            foreach (var actor in actors)
            {
                var equipmentCarrier = actor.Person.EquipmentCarrier;
                if (equipmentCarrier == null)
                {
                    continue;
                }

                foreach (var equipment in equipmentCarrier)
                {
                    if (equipment == null)
                    {
                        // пустой слот.
                        continue;
                    }

                    _equipmentDurableService.UpdateByTurn(equipment, actor.Person);
                }
            }
        }

        /// <summary>
        /// Определяет, находятся ли актёры игрока в точках выхода их сектора.
        /// </summary>
        private void DetectSectorExit()
        {
            var humanActorNodes = _actorManager.Items.Where(x => x.Owner is HumanPlayer).Select(x => x.Node);
            var detectedTransition = TransitionDetection.Detect(Map.Transitions, humanActorNodes);

            if (detectedTransition != null)
            {
                DoActorExit(detectedTransition);
            }
        }


        private void PropContainerManager_Added(object sender, ManagerItemsChangedEventArgs<IPropContainer> e)
        {
            foreach (var container in e.Items)
            {
                if (container.IsMapBlock)
                {
                    Map.HoldNode(container.Node, container);
                }

                if (container is ILootContainer)
                {
                    container.ItemsRemoved += LootContainer_ItemsRemoved;
                }
            }
        }

        private void LootContainer_ItemsRemoved(object sender, PropStoreEventArgs e)
        {
            var container = (IPropContainer)sender;
            if (!container.Content.CalcActualItems().Any())
            {
                _propContainerManager.Remove(container);
            }
        }

        private void PropContainerManager_Remove(object sender, ManagerItemsChangedEventArgs<IPropContainer> e)
        {
            foreach (var container in e.Items)
            {
                if (container.IsMapBlock)
                {
                    Map.ReleaseNode(container.Node, container);
                }

                if (container is ILootContainer)
                {
                    container.ItemsRemoved -= LootContainer_ItemsRemoved;
                }
            }
        }

        private void ActorManager_Added(object sender, ManagerItemsChangedEventArgs<IActor> e)
        {
            foreach (var actor in e.Items)
            {
                Map.HoldNode(actor.Node, actor);

                actor.Person.Survival.Dead += ActorState_Dead;

                if (actor.Owner is HumanPlayer)
                {
                    actor.Moved += HumanActor_Moved;
                }
            }
        }

        private void HumanActor_Moved(object sender, EventArgs e)
        {
            if (ScoreManager != null)
            {
                ScoreManager.CountTurn(Scheme);
            }
        }

        private void ActorState_Dead(object sender, EventArgs e)
        {
            var actor = _actorManager.Items.Single(x => x.Person.Survival == sender);
            Map.ReleaseNode(actor.Node, actor);
            _actorManager.Remove(actor);
            actor.Person.Survival.Dead -= ActorState_Dead;
            actor.Moved -= HumanActor_Moved;
            ProcessMonsterDeath(actor);
        }

        private void ProcessMonsterDeath(IActor actor)
        {
            if (!(actor.Person is MonsterPerson monsterPerson))
            {
                return;
            }

            var monsterScheme = monsterPerson.Scheme;

            var dropSchemes = GetMonsterDropTables(monsterScheme);

            var loot = new DropTableLoot(actor.Node, dropSchemes, _dropResolver);

            if (loot.Content.CalcActualItems().Any())
            {
                _propContainerManager.Add(loot);
            }

            if (ScoreManager != null)
            {
                ScoreManager.CountMonsterDefeat(monsterPerson);
            }
        }

        private IDropTableScheme[] GetMonsterDropTables(IMonsterScheme monsterScheme)
        {
            if (monsterScheme.DropTableSids == null)
            {
                return new IDropTableScheme[0];
            }

            var dropTableCount = monsterScheme.DropTableSids.Length;
            var schemes = new IDropTableScheme[dropTableCount];
            for (var i = 0; i < dropTableCount; i++)
            {
                var sid = monsterScheme.DropTableSids[i];
                schemes[i] = _schemeService.GetScheme<IDropTableScheme>(sid);
            }

            return schemes;
        }

        private void DoActorExit([NotNull] RoomTransition roomTransition)
        {
            var e = new SectorExitEventArgs(roomTransition);
            HumanGroupExit?.Invoke(this, e);
        }
    }
}
