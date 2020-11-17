using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Diseases;
using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics.Behaviour;
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
        private const int NATIONALUNITYCOUNTERSTARTVALUE = 1000;

        private readonly List<IDisease> _diseases;
        private readonly IDropResolver _dropResolver;
        private readonly IEquipmentDurableService _equipmentDurableService;
        private readonly ISchemeService _schemeService;
        private int _nationalUnityCounter = NATIONALUNITYCOUNTERSTARTVALUE;

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
            _equipmentDurableService = equipmentDurableService ??
                                       throw new ArgumentNullException(nameof(equipmentDurableService));

            _diseases = new List<IDisease>();

            ActorManager.Added += ActorManager_Added;
            ActorManager.Removed += ActorManager_Remove;
            StaticObjectManager.Added += StaticObjectManager_Added;
            StaticObjectManager.Removed += StaticObjectManager_Remove;

            Map = map ?? throw new ArgumentException("Не передана карта сектора.", nameof(map));

            PatrolRoutes = new Dictionary<IActor, IPatrolRoute>();
        }

        public NationalUnityEventService NationalUnityEventService { get; set; }

        public string Sid { get; set; }

        /// <summary>
        /// Стартовые узлы.
        /// Набор узлов, где могут располагаться актёры игрока
        /// на начало прохождения сектора.
        /// </summary>
        public IGraphNode[] StartNodes { get; set; }

        private void Actor_Moved(object sender, EventArgs e)
        {
            var actor = (IActor)sender;
            UpdateFowData(actor);
        }

        private void ActorManager_Added(object sender, ManagerItemsChangedEventArgs<IActor> e)
        {
            foreach (var actor in e.Items)
            {
                HoldNodes(actor.Node, actor, Map);

                if (actor.Person.GetModuleSafe<ISurvivalModule>() != null)
                {
                    actor.Person.GetModule<ISurvivalModule>().Dead += ActorState_Dead;
                }

                actor.Moved += Actor_Moved;
                UpdateFowData(actor);
            }
        }

        private void ActorManager_Remove(object sender, ManagerItemsChangedEventArgs<IActor> e)
        {
            // Когда актёры удалены из сектора, мы перестаём мониторить события на них.
            foreach (var actor in e.Items)
            {
                ReleaseNodes(actor, Map);

                if (actor.Person.GetModuleSafe<ISurvivalModule>() != null)
                {
                    actor.Person.GetModule<ISurvivalModule>().Dead -= ActorState_Dead;
                }

                actor.Moved -= Actor_Moved;
            }
        }

        private void ActorState_Dead(object sender, EventArgs e)
        {
            var actor = ActorManager.Items.Single(x =>
                ReferenceEquals(x.Person.GetModuleSafe<ISurvivalModule>(), sender));
            ActorManager.Remove(actor);

            if (actor.Person.GetModuleSafe<ISurvivalModule>() != null)
            {
                actor.Person.GetModule<ISurvivalModule>().Dead -= ActorState_Dead;
            }

            ProcessMonsterDeath(actor);
        }

        private void DoActorExit([NotNull] IActor actor, [NotNull] RoomTransition roomTransition)
        {
            var e = new TransitionUsedEventArgs(actor, roomTransition);
            TrasitionUsed?.Invoke(this, e);
        }

        private static IEnumerable<IGraphNode> GetActorNodes(PhysicalSizePattern physicalSize, IGraphNode baseNode,
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

        private static void HoldNodes(IGraphNode nextNode, IActor actor, IMap map)
        {
            var actorNodes = GetActorNodes(actor.Person.PhysicalSize, nextNode, map);

            foreach (var node in actorNodes)
            {
                map.HoldNode(node, actor);
            }
        }

        private void LootContainer_ItemsRemoved(object sender, PropStoreEventArgs e)
        {
            var container = (IPropContainer)sender;
            if (!container.Content.CalcActualItems().Any())
            {
                var staticObject =
                    StaticObjectManager.Items.Single(x =>
                        ReferenceEquals(x.GetModuleSafe<IPropContainer>(), container));
                StaticObjectManager.Remove(staticObject);
            }
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

            var loot = new DropTableLoot(dropSchemes, _dropResolver);

            var staticObject = new StaticObject(actor.Node, loot.Purpose, default);
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

        private static void ReleaseNodes(IActor actor, IMap map)
        {
            var actorNodes = GetActorNodes(actor.Person.PhysicalSize, actor.Node, map);

            foreach (var node in actorNodes)
            {
                map.ReleaseNode(node, actor);
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

        private void UpdateActorCombatActs()
        {
            foreach (var actor in ActorManager.Items.ToArray())
            {
                var combatActModule = actor.Person?.GetModuleSafe<ICombatActModule>();
                if (combatActModule is null)
                {
                    continue;
                }

                var combatActs = combatActModule.CalcCombatActs();

                foreach (var act in combatActs)
                {
                    act.UpdateCooldown();
                }
            }
        }

        private void UpdateActorEffects()
        {
            foreach (var actor in ActorManager.Items.ToArray())
            {
                var effects = actor.Person.GetModuleSafe<IEffectsModule>();

                if (effects is null)
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
                    if (effect is ISurvivalStatEffect actorEffect &&
                        actor.Person.GetModuleSafe<ISurvivalModule>() != null)
                    {
                        actorEffect.Apply(actor.Person.GetModule<ISurvivalModule>());
                    }
                }
            }
        }

        private void UpdateDiseases()
        {
            foreach (var actor in ActorManager.Items.ToArray())
            {
                if (actor.Person.GetModuleSafe<IDiseaseModule>() is null)
                {
                    continue;
                }

                actor.Person.GetModule<IDiseaseModule>().Update(actor.Person.GetModuleSafe<IEffectsModule>());
            }
        }

        private void UpdateEquipments()
        {
            var actors = ActorManager.Items.ToArray();
            foreach (var actor in actors)
            {
                var equipmentCarrier = actor.Person.GetModuleSafe<IEquipmentModule>();
                if (equipmentCarrier is null)
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

        private void UpdateFowData(IActor actor)
        {
            var fowModule = actor.Person.GetModuleSafe<IFowData>();
            if (fowModule != null)
            {
                var fowData = fowModule.GetSectorFowData(this);
                const int DISTANCE_OF_SIGN = 5;
                var fowContext = new FowContext(Map, StaticObjectManager);
                FowHelper.UpdateFowData(fowData, fowContext, actor.Node, DISTANCE_OF_SIGN);
            }
        }

        /// <summary>
        /// Processing special event:
        /// 1. There is counter.
        /// 2. When counter is out the special service create group of interventionists or militia.
        /// </summary>
        private void UpdateNationalUnityEvent()
        {
            if (NationalUnityEventService is null)
            {
                return;
            }

            _nationalUnityCounter--;
            if (_nationalUnityCounter <= 0)
            {
                if (NationalUnityEventService.RollEventIsRaised())
                {
                    NationalUnityEventService.RollAndCreateUnityGroupIntoSector(this);
                }

                _nationalUnityCounter = NATIONALUNITYCOUNTERSTARTVALUE;
            }
        }

        private void UpdateScores()
        {
            if (ScoreManager != null)
            {
                ScoreManager.CountTurn(Scheme);
            }
        }

        private void UpdateSurvivals()
        {
            var actors = ActorManager.Items.ToArray();
            foreach (var actor in actors)
            {
                var survival = actor.Person.GetModuleSafe<ISurvivalModule>();
                if (survival == null)
                {
                    continue;
                }

                survival.Update();
            }
        }

        /// <summary>
        /// Событие выстреливает, когда группа актёров игрока покинула сектор.
        /// </summary>
        public event EventHandler<TransitionUsedEventArgs> TrasitionUsed;

        /// <summary>
        /// Карта в основе сектора.
        /// </summary>
        public ISectorMap Map { get; }

        /// <summary>
        /// Маршруты патрулирования в секторе.
        /// </summary>
        public Dictionary<IActor, IPatrolRoute> PatrolRoutes { get; }

        /// <summary>
        /// Менеджер работы с очками.
        /// </summary>
        public IScoreManager ScoreManager { get; set; }

        public ILocationScheme Scheme { get; set; }
        public IActorManager ActorManager { get; }
        public IStaticObjectManager StaticObjectManager { get; }
        public IEnumerable<IDisease> Diseases => _diseases;

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

            UpdateActorCombatActs();

            UpdateNationalUnityEvent();
        }

        public void UseTransition(IActor actor, RoomTransition transition)
        {
            DoActorExit(actor, transition);
        }
    }
}