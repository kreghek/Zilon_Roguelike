using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Diseases;
using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Базовая реализация сектора.
    /// </summary>
    /// <seealso cref="ISector" />
    public class Sector : ISector
    {
        private readonly IDropResolver _dropResolver;
        private readonly ISchemeService _schemeService;
        private readonly IEquipmentDurableService _equipmentDurableService;

        private readonly List<IDisease> _diseases;

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
        public IGraphNode[] StartNodes { get; set; }

        /// <summary>
        /// Менеджер работы с очками.
        /// </summary>
        public IScoreManager ScoreManager { get; set; }

        public string Sid { get; set; }

        public ILocationScheme Scheme { get; set; }
        public IActorManager ActorManager { get; }
        public IStaticObjectManager StaticObjectManager { get; }
        public IEnumerable<IDisease> Diseases { get => _diseases; }

        [ExcludeFromCodeCoverage]
        public Sector(ISectorMap map,
            IActorManager actorManager,
            IStaticObjectManager staticObjectManager,
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IEquipmentDurableService equipmentDurableService)
        {
            ActorManager = actorManager ?? throw new ArgumentNullException(nameof(actorManager));
            StaticObjectManager = staticObjectManager ?? throw new ArgumentNullException(nameof(staticObjectManager));
            _dropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _equipmentDurableService = equipmentDurableService ?? throw new ArgumentNullException(nameof(equipmentDurableService));

            _diseases = new List<IDisease>();

            ActorManager.Added += ActorManager_Added;
            ActorManager.Removed += ActorManager_Remove;
            StaticObjectManager.Added += StaticObjectManager_Added;
            StaticObjectManager.Removed += StaticObjectManager_Remove;

            Map = map ?? throw new ArgumentException("Не передана карта сектора.", nameof(map));

            PatrolRoutes = new Dictionary<IActor, IPatrolRoute>();
        }

        public void AddDisease(IDisease disease)
        {
            _diseases.Add(disease);
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
            UpdateScores();

            UpdateSurvivals();

            UpdateActorEffects();

            UpdateDiseases();

            UpdateEquipments();

            UpdateActorActs();
        }

        private void UpdateDiseases()
        {
            foreach (var actor in ActorManager.Items.ToArray())
            {
                if (actor.Person.DiseaseData is null)
                {
                    continue;
                }

                actor.Person.DiseaseData.Update(actor.Person.Effects);
            }
        }

        private void UpdateActorActs()
        {
            foreach (var actor in ActorManager.Items.ToArray())
            {
                if (actor.Person?.TacticalActCarrier?.Acts is null)
                {
                    continue;
                }

                foreach (var act in actor.Person.TacticalActCarrier.Acts)
                {
                    act.UpdateCooldown();
                }
            }
        }

        private void UpdateScores()
        {
            if (ScoreManager != null)
            {
                ScoreManager.CountTurn(Scheme);
            }
        }

        private void UpdateActorEffects()
        {
            foreach (var actor in ActorManager.Items.ToArray())
            {
                var effects = actor.Person.Effects;

                if (effects == null)
                {
                    continue;
                }

                //TODO Выяснить, нужен ли ToArray() в effects.Items
                // Было добавлено, потому что в результате запуска одного из тестов для ботов
                // здесь выпало исключение, то коллекция изменилась.
                // Но раньше никогда этой ошибки не было ни в тестах, ни на клиенте.
                // Чтобы решить этот TODO, необходимо подобрать набор тестов, в результате которых
                // Items изменяется. Они должны падать, если убрать ToArray и выполняться, если его вернуть.
                foreach (var effect in effects.Items.ToArray())
                {
                    if (effect is ISurvivalStatEffect actorEffect && actor.Person.Survival != null)
                    {
                        actorEffect.Apply(actor.Person.Survival);
                    }
                }
            }
        }

        private void UpdateSurvivals()
        {
            var actors = ActorManager.Items.ToArray();
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
            var actors = ActorManager.Items.ToArray();
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

        private void StaticObjectManager_Added(object sender, ManagerItemsChangedEventArgs<IStaticObject> e)
        {
            foreach (var container in e.Items)
            {
                if (container.IsMapBlock)
                {
                    Map.HoldNode(container.Node, container);
                }

                if (container.GetModuleSafe<IPropContainer>() is ILootContainer lootContainer)
                {
                    lootContainer.ItemsRemoved += LootContainer_ItemsRemoved;
                }
            }
        }

        private void LootContainer_ItemsRemoved(object sender, PropStoreEventArgs e)
        {
            var container = (IPropContainer)sender;
            if (!container.Content.CalcActualItems().Any())
            {
                var staticObject = StaticObjectManager.Items.Single(x=>ReferenceEquals(x.GetModuleSafe<IPropContainer>(), container));
                StaticObjectManager.Remove(staticObject);
            }
        }

        private void StaticObjectManager_Remove(object sender, ManagerItemsChangedEventArgs<IStaticObject> e)
        {
            foreach (var container in e.Items)
            {
                if (container.IsMapBlock)
                {
                    Map.ReleaseNode(container.Node, container);
                }

                if (container.GetModule<IPropContainer>() is ILootContainer lootContainer)
                {
                    lootContainer.ItemsRemoved -= LootContainer_ItemsRemoved;
                }
            }
        }

        private void ActorManager_Added(object sender, ManagerItemsChangedEventArgs<IActor> e)
        {
            foreach (var actor in e.Items)
            {
                HoldNodes(actor.Node, actor, Map);

                if (actor.Person.Survival != null)
                {
                    actor.Person.Survival.Dead += ActorState_Dead;
                }
            }
        }

        private void HoldNodes(IGraphNode nextNode, IActor actor, IMap map)
        {
            var actorNodes = GetActorNodes(actor.Person.PhysicalSize, nextNode, map);

            foreach (var node in actorNodes)
            {
                map.HoldNode(node, actor);
            }
        }

        private void ReleaseNodes(IActor actor, IMap map)
        {
            var actorNodes = GetActorNodes(actor.Person.PhysicalSize, actor.Node,  map);

            foreach (var node in actorNodes)
            {
                map.ReleaseNode(node, actor);
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

        private void ActorManager_Remove(object sender, ManagerItemsChangedEventArgs<IActor> e)
        {
            // Когда актёры удалены из сектора, мы перестаём мониторить события на них.
            foreach (var actor in e.Items)
            {
                ReleaseNodes(actor, Map);

                if (actor.Person.Survival != null)
                {
                    actor.Person.Survival.Dead -= ActorState_Dead;
                }
            }
        }

        private void ActorState_Dead(object sender, EventArgs e)
        {
            var actor = ActorManager.Items.Single(x => x.Person.Survival == sender);
            ActorManager.Remove(actor);

            if (actor.Person.Survival != null)
            {
                actor.Person.Survival.Dead -= ActorState_Dead;
            }

            ProcessMonsterDeath(actor);
        }

        private void ProcessMonsterDeath(IActor actor)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (!(actor.Person is MonsterPerson monsterPerson))
            {
                return;
            }

            var monsterScheme = monsterPerson.Scheme;

            var dropSchemes = GetMonsterDropTables(monsterScheme);

            var loot = new DropTableLoot(actor.Node, dropSchemes, _dropResolver);

            var staticObject = new StaticObject(actor.Node, default);
            staticObject.AddModule<IPropContainer>(loot);

            if (loot.Content.CalcActualItems().Any())
            {
                StaticObjectManager.Add(staticObject);
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
                return Array.Empty<IDropTableScheme>();
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

        public void UseTransition(RoomTransition transition)
        {
            DoActorExit(transition);
        }
    }
}
