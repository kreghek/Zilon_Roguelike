using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.MapGenerators;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

// ReSharper disable once CheckNamespace
// ReSharper disable once ArrangeTypeModifiers
// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once UnusedMember.Global
internal class SectorVM : MonoBehaviour
{

    private readonly List<MapNodeVM> _nodeViewModels;
    private readonly List<ActorViewModel> _actorViewModels;
    private readonly List<ContainerVm> _containerViewModels;

#pragma warning disable 649
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable NotNullMemberIsNotInitialized
    [NotNull] public MapNodeVM MapNodePrefab;

    [NotNull] public ActorViewModel ActorPrefab;

    [NotNull] public GunShootTracer GunShootTracer;

    [NotNull] public HumanoidActorGraphic HumanoidGraphicPrefab;

    [NotNull] public MonoActorGraphic MonoGraphicPrefab;

    [NotNull] public ContainerVm ChestPrefab;

    [NotNull] public ContainerVm LootPrefab;

    [NotNull] public HitSfx HitSfx;

    [NotNull] [Inject] private readonly DiContainer _container;

    [NotNull] [Inject] private readonly IGameLoop _gameLoop;

    [NotNull] [Inject] private readonly ICommandManager _clientCommandExecutor;

    [NotNull] [Inject] private readonly ISectorManager _sectorManager;

    [NotNull] [Inject] private readonly IPlayerState _playerState;

    [NotNull] [Inject] private readonly ISchemeService _schemeService;

    [NotNull] [Inject] private readonly IPropFactory _propFactory;

    [NotNull] [Inject] private readonly HumanPlayer _humanPlayer;

    [NotNull] [Inject] private readonly IActorManager _actorManager;

    [NotNull] [Inject] private readonly IPropContainerManager _propContainerManager;

    [NotNull] [Inject] private readonly IHumanPersonManager _personManager;

    [NotNull] [Inject] private readonly ISectorModalManager _sectorModalManager;

    [NotNull] [Inject] private readonly ISurvivalRandomSource _survivalRandomSource;

    [Inject] private IHumanActorTaskSource _humanActorTaskSource;

    [Inject(Id = "monster")] private readonly IActorTaskSource _monsterActorTaskSource;

    [Inject] private readonly IBotPlayer _botPlayer;

    [NotNull]
    [Inject(Id = "move-command")]
    private readonly ICommand _moveCommand;

    [NotNull]
    [Inject(Id = "attack-command")]
    private readonly ICommand _attackCommand;

    [NotNull]
    [Inject(Id = "open-container-command")]
    private readonly ICommand _openContainerCommand;

    [NotNull]
    [Inject(Id = "show-container-modal-command")]
    private readonly ICommand _showContainerModalCommand;

    public SectorVM()
    {
        _nodeViewModels = new List<MapNodeVM>();
        _actorViewModels = new List<ActorViewModel>();
        _containerViewModels = new List<ContainerVm>();
    }

    // ReSharper restore NotNullMemberIsNotInitialized
    // ReSharper restore MemberCanBePrivate.Global
#pragma warning restore 649

    // ReSharper disable once UnusedMember.Local
    private void FixedUpdate()
    {
        ExecuteCommands();
    }

    private void ExecuteCommands()
    {
        var command = _clientCommandExecutor.Pop();

        try
        {
            command?.Execute();
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Не удалось выполнить команду {command}.", exception);
        }
    }

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        InitServices();

        var nodeViewModels = InitNodeViewModels();
        _nodeViewModels.AddRange(nodeViewModels);

        InitPlayerActor(nodeViewModels);
        CreateMonsterViewModels(nodeViewModels);
        CreateContainerViewModels(nodeViewModels);

        if (_personManager.SectorLevel == 0)
        {
            _sectorModalManager.ShowInstructionModal();
        }
    }

    private void PropContainerManager_Added(object sender, ManagerItemsChangedEventArgs<IPropContainer> e)
    {
        foreach (var container in e.Items)
        {
            CreateContainerViewModel(_nodeViewModels, container);
        }
    }

    private void InitServices()
    {
        var proceduralGeneratorOptions = CreateSectorGeneratorOptions();

        _sectorManager.CreateSector(proceduralGeneratorOptions);

        _propContainerManager.Added += PropContainerManager_Added;
        _propContainerManager.Removed += PropContainerManager_Removed;

        _playerState.TaskSource = _humanActorTaskSource;

        _gameLoop.ActorTaskSources = new[] {
            _humanActorTaskSource,
            _monsterActorTaskSource
        };

        _sectorManager.CurrentSector.ActorExit += SectorOnActorExit;
    }

    private ISectorGeneratorOptions CreateSectorGeneratorOptions()
    {
        var monsterGeneratorOptions = new MonsterGeneratorOptions
        {
            BotPlayer = _botPlayer
        };

        var proceduralGeneratorOptions = new SectorProceduralGeneratorOptions
        {
            MonsterGeneratorOptions = monsterGeneratorOptions
        };

        var wellFormedSectorLevel = _personManager.SectorLevel + 1;

        switch (wellFormedSectorLevel)
        {
            case 1:
                monsterGeneratorOptions.RegularMonsterSids = new[] { "rat", "bat" };
                monsterGeneratorOptions.RareMonsterSids = new[] { "rat-mutant", "moon-rat" };
                monsterGeneratorOptions.ChampionMonsterSids = new[] { "rat-king", "rat-human-slayer", "night-stalker" };
                break;

            case 2:
                monsterGeneratorOptions.RegularMonsterSids = new[] { "genomass", "gemonass-slave" };
                monsterGeneratorOptions.RareMonsterSids = new[] { "infernal-genomass", "dervish" };
                monsterGeneratorOptions.ChampionMonsterSids = new[] { "necromancer" };
                break;

            case 3:
                monsterGeneratorOptions.RegularMonsterSids = new[] { "skeleton-grunt", "skeleton-warrior", "zombie", "grave-worm", "ghoul" };
                monsterGeneratorOptions.RareMonsterSids = new[] { "skeleton-champion", "vampire" };
                monsterGeneratorOptions.ChampionMonsterSids = new[] { "necromancer", "demon-roamer" };
                break;

            case 4:
                monsterGeneratorOptions.RegularMonsterSids = new[] { "demon", "demon-spearman", "demon-bat", "hell-bat" };
                monsterGeneratorOptions.RareMonsterSids = new[] { "demon-warlock", "hell-rock", "infernal-bard" };
                monsterGeneratorOptions.ChampionMonsterSids = new[] { "demon-lord", "archidemon", "hell-herald", "pit-baron" };
                break;

            case 5:
                monsterGeneratorOptions.RegularMonsterSids = new[] { "elder-slave", "dark-guardian" };
                monsterGeneratorOptions.RareMonsterSids = new[] { "eternal-executor", "dark-seer" };
                monsterGeneratorOptions.ChampionMonsterSids = new[] { "manticore" };
                break;
        }

        return proceduralGeneratorOptions;
    }

    private void PropContainerManager_Removed(object sender, ManagerItemsChangedEventArgs<IPropContainer> e)
    {
        foreach (var container in e.Items)
        {
            var containerViewModel = _containerViewModels.Single(x => x.Container == container);
            _containerViewModels.Remove(containerViewModel);
            Destroy(containerViewModel.gameObject);
        }
    }

    public void OnDestroy()
    {
        _propContainerManager.Added -= PropContainerManager_Added;
        _propContainerManager.Removed -= PropContainerManager_Removed;
        _sectorManager.CurrentSector.ActorExit -= SectorOnActorExit;
    }

    private void InitPlayerActor(IEnumerable<MapNodeVM> nodeViewModels)
    {
        var personScheme = _schemeService.GetScheme<IPersonScheme>("human-person");

        var playerActorStartNode = _sectorManager.CurrentSector.Map.StartNodes.First();
        var playerActorViewModel = CreateHumanActorVm(_humanPlayer,
            personScheme,
            _actorManager,
            _survivalRandomSource,
            playerActorStartNode,
            nodeViewModels);

        //Лучше централизовать переключение текущего актёра только в playerState
        _playerState.ActiveActor = playerActorViewModel;
        _humanActorTaskSource.SwitchActor(_playerState.ActiveActor.Actor);

        _actorViewModels.Add(playerActorViewModel);
    }

    private List<MapNodeVM> InitNodeViewModels()
    {
        var map = _sectorManager.CurrentSector.Map;
        var nodeVMs = new List<MapNodeVM>();
        foreach (var node in map.Nodes)
        {
            var mapNodeVm = Instantiate(MapNodePrefab, transform);

            var hexNode = (HexNode)node;
            var nodeWorldPositionParts = HexHelper.ConvertToWorld(hexNode.OffsetX, hexNode.OffsetY);
            var worldPosition = new Vector3(nodeWorldPositionParts[0], nodeWorldPositionParts[1] / 2);
            mapNodeVm.transform.position = worldPosition;
            mapNodeVm.Node = hexNode;
            mapNodeVm.Neighbors = map.GetNext(node).Cast<HexNode>().ToArray();

            if (map.ExitNodes?.Contains(node) == true)
            {
                mapNodeVm.IsExit = true;
            }

            mapNodeVm.OnSelect += MapNodeVm_OnSelect;

            nodeVMs.Add(mapNodeVm);
        }

        return nodeVMs;
    }

    private void CreateMonsterViewModels(IEnumerable<MapNodeVM> nodeViewModels)
    {
        var monsters = _actorManager.Items.Where(x => x.Person is MonsterPerson).ToArray();
        foreach (var monsterActor in monsters)
        {
            var actorViewModelObj = _container.InstantiatePrefab(ActorPrefab, transform);
            var actorViewModel = actorViewModelObj.GetComponent<ActorViewModel>();

            var actorGraphic = Instantiate(MonoGraphicPrefab, actorViewModel.transform);
            actorViewModel.GraphicRoot = actorGraphic;
            actorGraphic.transform.position = new Vector3(0, /*0.2f*/0, 0);

            var graphicController = actorViewModel.gameObject.AddComponent<MonsterSingleActorGraphicController>();
            graphicController.Actor = monsterActor;
            graphicController.Graphic = actorGraphic;

            var actorNodeVm = nodeViewModels.Single(x => x.Node == monsterActor.Node);
            var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
            actorViewModel.transform.position = actorPosition;
            actorViewModel.Actor = monsterActor;

            actorViewModel.Selected += EnemyActorVm_OnSelected;
            monsterActor.UsedAct += ActorOnUsedAct;

            _actorViewModels.Add(actorViewModel);
        }
    }

    private void CreateContainerViewModels(IEnumerable<MapNodeVM> nodeViewModels)
    {
        foreach (var container in _propContainerManager.Items)
        {
            CreateContainerViewModel(nodeViewModels, container);
        }
    }

    private void CreateContainerViewModel(IEnumerable<MapNodeVM> nodeViewModels, IPropContainer container)
    {
        var containerPrefab = GetContainerPrefab(container);

        var containerViewModel = Instantiate(containerPrefab, transform);

        var containerNodeVm = nodeViewModels.Single(x => x.Node == container.Node);
        var containerPosition = containerNodeVm.transform.position + new Vector3(0, 0, -1);
        containerViewModel.transform.position = containerPosition;
        containerViewModel.Container = container;
        containerViewModel.Selected += Container_Selected;

        _containerViewModels.Add(containerViewModel);
    }

    private ContainerVm GetContainerPrefab(IPropContainer container)
    {
        if (container is ILootContainer lootContainer)
        {
            return LootPrefab;
        }

        return ChestPrefab;
    }

    private void Container_Selected(object sender, EventArgs e)
    {
        var containerViewModel = sender as ContainerVm;

        _playerState.HoverViewModel = containerViewModel;

        if (containerViewModel != null)
        {
            _clientCommandExecutor.Push(_openContainerCommand);
        }
    }

    private void PlayerActorOnOpenedContainer(object sender, OpenContainerEventArgs e)
    {
        _clientCommandExecutor.Push(_showContainerModalCommand);
    }

    private void SectorOnActorExit(object sender, EventArgs e)
    {
        _playerState.ActiveActor = null;
        _humanActorTaskSource.SwitchActor(null);
        _personManager.SectorLevel++;

        if (_personManager.SectorLevel > 4)
        {
            _sectorModalManager.ShowWinModal();
        }
        else
        {
            SceneManager.LoadScene("combat");
        }
    }

    private void EnemyActorVm_OnSelected(object sender, EventArgs e)
    {
        if (_playerState.ActiveActor == null)
        {
            return;
        }

        var actorVm = sender as ActorViewModel;

        _playerState.HoverViewModel = actorVm;

        if (actorVm != null)
        {
            _clientCommandExecutor.Push(_attackCommand);
        }
    }

    private ActorViewModel CreateHumanActorVm([NotNull] IPlayer player,
        [NotNull] IPersonScheme personScheme,
        [NotNull] IActorManager actorManager,
        [NotNull] ISurvivalRandomSource survivalRandomSource,
        [NotNull] IMapNode startNode,
        [NotNull] IEnumerable<MapNodeVM> nodeVMs)
    {
        if (_personManager.Person == null)
        {
            var inventory = new Inventory();

            var evolutionData = new EvolutionData(_schemeService);

            var defaultActScheme = _schemeService.GetScheme<ITacticalActScheme>(personScheme.DefaultAct);

            var person = new HumanPerson(personScheme, defaultActScheme, evolutionData, survivalRandomSource, inventory);

            _personManager.Person = person;

            _personManager.SectorName = GetRandomName();

            var classRoll = UnityEngine.Random.Range(1, 6);
            switch (classRoll)
            {
                case 1:
                    AddEquipmentToActor(person.EquipmentCarrier, 2, "short-sword");
                    AddEquipmentToActor(person.EquipmentCarrier, 1, "steel-armor");
                    AddEquipmentToActor(person.EquipmentCarrier, 3, "wooden-shield");
                    break;

                case 2:
                    AddEquipmentToActor(person.EquipmentCarrier, 2, "battle-axe");
                    AddEquipmentToActor(person.EquipmentCarrier, 3, "battle-axe");
                    AddEquipmentToActor(person.EquipmentCarrier, 0, "steel-helmet");
                    break;

                case 3:
                    AddEquipmentToActor(person.EquipmentCarrier, 2, "bow");
                    AddEquipmentToActor(person.EquipmentCarrier, 1, "leather-armor");
                    AddEquipmentToActor(inventory, "short-sword");
                    AddResourceToActor(inventory, "arrow", 10);
                    break;

                case 4:
                    AddEquipmentToActor(person.EquipmentCarrier, 2, "fireball-staff");
                    AddEquipmentToActor(person.EquipmentCarrier, 1, "scholar-robe");
                    AddEquipmentToActor(person.EquipmentCarrier, 0, "wizard-hat");
                    AddResourceToActor(inventory, "mana", 15);
                    break;

                case 5:
                    AddEquipmentToActor(person.EquipmentCarrier, 2, "pistol");
                    AddEquipmentToActor(person.EquipmentCarrier, 0, "elder-hat");
                    AddResourceToActor(inventory, "bullet-45", 5);

                    AddResourceToActor(inventory, "packed-food", 1);
                    AddResourceToActor(inventory, "bottle", 1);
                    AddResourceToActor(inventory, "med-kit", 1);

                    AddResourceToActor(inventory, "mana", 5);
                    AddResourceToActor(inventory, "arrow", 3);
                    break;
            }

            AddResourceToActor(inventory, "packed-food", 1);
            AddResourceToActor(inventory, "bottle", 1);
            AddResourceToActor(inventory, "med-kit", 1);
        }

        var actor = new Actor(_personManager.Person, player, startNode);

        actorManager.Add(actor);

        var actorViewModelObj = _container.InstantiatePrefab(ActorPrefab, transform);
        var actorViewModel = actorViewModelObj.GetComponent<ActorViewModel>();
        var actorGraphic = Instantiate(HumanoidGraphicPrefab, actorViewModel.transform);
        actorGraphic.transform.position = new Vector3(0, 0.2f, 0);
        actorViewModel.GraphicRoot = actorGraphic;

        var graphicController = actorViewModel.gameObject.AddComponent<HumanActorGraphicController>();
        graphicController.Actor = actor;
        graphicController.Graphic = actorGraphic;

        var actorNodeVm = nodeVMs.Single(x => x.Node == actor.Node);
        var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
        actorViewModel.transform.position = actorPosition;
        actorViewModel.Actor = actor;

        actorViewModel.Actor.OpenedContainer += PlayerActorOnOpenedContainer;
        actorViewModel.Actor.UsedAct += ActorOnUsedAct;

        return actorViewModel;
    }

    private string GetRandomName()
    {
        var names = new[] { "Dungeon", "Caves", "Catacombs", "Ruins", "Chirch" };
        return names[UnityEngine.Random.Range(0, names.Length - 1)];
    }

    private void AddEquipmentToActor(Inventory inventory, string equipmentSid)
    {
        try
        {
            var equipmentScheme = _schemeService.GetScheme<IPropScheme>(equipmentSid);
            var equipment = _propFactory.CreateEquipment(equipmentScheme);
            inventory.Add(equipment);
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError($"Не найден объект {equipmentSid}");
        }
    }

    private void AddEquipmentToActor(IEquipmentCarrier equipmentCarrier, int slotIndex, string equipmentSid)
    {
        try
        {
            var equipmentScheme = _schemeService.GetScheme<IPropScheme>(equipmentSid);
            var equipment = _propFactory.CreateEquipment(equipmentScheme);
            equipmentCarrier[slotIndex] = equipment;
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError($"Не найден объект {equipmentSid}");
        }
    }

    private void AddResourceToActor(Inventory inventory, string resourceSid, int count)
    {
        try
        {
            var resourceScheme = _schemeService.GetScheme<IPropScheme>(resourceSid);
            var resource = _propFactory.CreateResource(resourceScheme, count);
            inventory.Add(resource);
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError($"Не найден объект {resourceSid}");
        }
    }

    private void ActorOnUsedAct(object sender, UsedActEventArgs e)
    {
        var actor = GetActor(sender);

        var actorHexNode = actor.Node as HexNode;
        var targetHexNode = e.Target.Node as HexNode;

        // Визуализируем удар.
        var actorViewModel = _actorViewModels.Single(x => x.Actor == actor);
        actorViewModel.GraphicRoot.ProcessHit();

        var targetViewModel = _actorViewModels.Single(x => x.Actor == e.Target);

        var sfx = Instantiate(HitSfx, transform);
        targetViewModel.AddHitEffect(sfx);

        // Проверяем, стрелковое оружие или удар ближнего боя
        if (e.TacticalAct.Stats.Range.Max > 1)
        {
            sfx.EffectSpriteRenderer.sprite = sfx.ShootSprite;

            // Создаём снараяд
            CreateBullet(actor, e.Target);
        }
    }

    private static IActor GetActor(object sender)
    {
        if (sender is IActor actor)
        {
            return actor;
        }

        throw new NotSupportedException("Не поддерживается обработка событий использования действия.");
    }

    private void CreateBullet(IActor actor, IAttackTarget target)
    {
        var actorViewModel = _actorViewModels.Single(x => x.Actor == actor);
        var targetViewModel = _actorViewModels.Single(x => x.Actor == target);

        var bulletTracer = Instantiate(GunShootTracer, transform);
        bulletTracer.FromPosition = actorViewModel.transform.position;
        bulletTracer.TargetPosition = targetViewModel.transform.position;
    }

    private void MapNodeVm_OnSelect(object sender, EventArgs e)
    {
        if (_playerState.ActiveActor == null)
        {
            return;
        }

        // указываем намерение двигиться на выбранную точку (узел).

        var nodeVm = sender as MapNodeVM;

        _playerState.HoverViewModel = nodeVm;

        if (nodeVm != null)
        {
            _clientCommandExecutor.Push(_moveCommand);
        }
    }
}