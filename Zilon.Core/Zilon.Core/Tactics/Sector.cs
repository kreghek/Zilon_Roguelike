using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{

    public class Sector : ISector
    {
        private readonly IActorManager _actorManager;
        private readonly IPropContainerManager _propContainerManager;
        private readonly IDropResolver _dropResolver;
        private readonly ISchemeService _schemeService;

        public event EventHandler ActorExit;

        [ExcludeFromCodeCoverage]
        public IMapNode[] ExitNodes { get; set; }

        [ExcludeFromCodeCoverage]
        public IMap Map { get; }

        [ExcludeFromCodeCoverage]
        public Dictionary<IActor, IPatrolRoute> PatrolRoutes { get; }

        [ExcludeFromCodeCoverage]
        public IMapNode[] StartNodes { get; set; }

        [ExcludeFromCodeCoverage]
        public Sector(IMap map,
            IActorManager actorManager,
            IPropContainerManager propContainerManager,
            IDropResolver dropResolver,
            ISchemeService schemeService)
        {
            _actorManager = actorManager;
            _propContainerManager = propContainerManager;
            _dropResolver = dropResolver;
            _schemeService = schemeService;
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
                    if (effect is IActorStateEffect actorEffect)
                    {
                        actorEffect.Apply(actor.State);
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

        /// <summary>
        /// Определяет, находятся ли актёры игрока в точках выхода их сектора.
        /// </summary>
        private void DetectSectorExit()
        {
            var allExit = true;

            foreach (var actor in _actorManager.Items)
            {
                if (actor.Owner is HumanPlayer && !ExitNodes.Contains(actor.Node))
                {
                    allExit = false;
                }
            }

            if (allExit)
            {
                DoActorExit();
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
            }
        }

        private void ActorManager_Added(object sender, ManagerItemsChangedEventArgs<IActor> e)
        {
            foreach (var actor in e.Items)
            {
                Map.HoldNode(actor.Node, actor);

                actor.State.Dead += ActorState_Dead;
            }
        }

        private void ActorState_Dead(object sender, EventArgs e)
        {
            var actor = _actorManager.Items.Single(x => x.State == sender);
            Map.ReleaseNode(actor.Node, actor);
            _actorManager.Remove(actor);
            actor.State.Dead -= ActorState_Dead;

            if (actor.Person is MonsterPerson monsterPerson)
            {
                var monsterScheme = monsterPerson.Scheme;

                var dropSchemes = GetMonsterDropTables(monsterScheme);

                var loot = new DropTableLoot(actor.Node, dropSchemes, _dropResolver);

                if (loot.Content.CalcActualItems().Any())
                {
                    _propContainerManager.Add(loot);
                }
            }
        }

        private DropTableScheme[] GetMonsterDropTables(IMonsterScheme monsterScheme)
        {
            if (monsterScheme.DropTableSids == null)
            {
                return new DropTableScheme[0];
            }

            var dropTableCount = monsterScheme.DropTableSids.Length;
            var schemes = new DropTableScheme[dropTableCount];
            for (var i = 0; i < dropTableCount; i++)
            {
                var sid = monsterScheme.DropTableSids[i];
                schemes[i] = _schemeService.GetScheme<DropTableScheme>(sid);
            }

            return schemes;
        }

        private void DoActorExit()
        {
            var e = new EventArgs();
            ActorExit?.Invoke(this, e);
        }
    }
}
