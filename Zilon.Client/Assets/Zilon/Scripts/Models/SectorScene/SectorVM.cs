﻿using System;
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
    private readonly List<TraderViewModel> _traderViewModels;

    private bool _interuptCommands;

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

    [NotNull] public TraderViewModel TraderPrefab;

    [NotNull] public HitSfx HitSfx;

    [NotNull] [Inject] private readonly DiContainer _container;

    [NotNull] [Inject] private readonly IGameLoop _gameLoop;

    [NotNull] [Inject] private readonly ICommandManager _clientCommandExecutor;

    [NotNull] [Inject] private readonly ISectorManager _sectorManager;

    [NotNull] [Inject] private readonly ISectorGeneratorSelector _sectorGeneratorSelector;

    [NotNull] [Inject] private readonly IPlayerState _playerState;

    [NotNull] [Inject] private readonly ISchemeService _schemeService;

    [NotNull] [Inject] private readonly IPropFactory _propFactory;

    [NotNull] [Inject] private readonly HumanPlayer _humanPlayer;

    [NotNull] [Inject] private readonly IActorManager _actorManager;

    [NotNull] [Inject] private readonly IPropContainerManager _propContainerManager;

    [NotNull] [Inject] private readonly ITraderManager _traderManager;

    [NotNull] [Inject] private readonly IHumanPersonManager _personManager;

    [NotNull] [Inject] private readonly ISectorModalManager _sectorModalManager;

    [NotNull] [Inject] private readonly ISurvivalRandomSource _survivalRandomSource;

    [Inject] private IHumanActorTaskSource _humanActorTaskSource;

    [Inject(Id = "monster")] private readonly IActorTaskSource _monsterActorTaskSource;

    [Inject] private readonly IBotPlayer _botPlayer;

    [Inject] private readonly ICommandBlockerService _commandBlockerService;

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

    [NotNull]
    [Inject(Id = "show-trader-modal-command")]
    private readonly ICommand _showTraderModalCommand;

    public SectorVM()
    {
        _nodeViewModels = new List<MapNodeVM>();
        _actorViewModels = new List<ActorViewModel>();
        _containerViewModels = new List<ContainerVm>();
        _traderViewModels = new List<TraderViewModel>();
    }

    // ReSharper restore NotNullMemberIsNotInitialized
    // ReSharper restore MemberCanBePrivate.Global
#pragma warning restore 649

    // ReSharper disable once UnusedMember.Local
    private void FixedUpdate()
    {
        if (!_commandBlockerService.HasBlockers)
        {
            ExecuteCommands();
        }
    }

    private void ExecuteCommands()
    {
        var command = _clientCommandExecutor.Pop();

        try
        {
            if (command != null)
            {
                command.Execute();

                if (_interuptCommands)
                {
                    return;
                }

                if (command is IRepeatableCommand repeatableCommand)
                {
                    if (repeatableCommand.CanRepeat())
                    {
                        _clientCommandExecutor.Push(repeatableCommand);
                    }
                }
            }
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
        CreateTraderViewModels(nodeViewModels);

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

        ISectorGeneratorOptions proceduralGeneratorOptions;

        if (_humanPlayer.GlobeNode == null)
        {
            var introLocationScheme = _schemeService.GetScheme<ILocationScheme>("intro");
            proceduralGeneratorOptions = CreateSectorGeneratorOptions(introLocationScheme);
        }
        else
        {
            var currentLocation = _humanPlayer.GlobeNode.Scheme;
            proceduralGeneratorOptions = CreateSectorGeneratorOptions(currentLocation);
        }

        var sectorGenerator = _sectorGeneratorSelector.GetGenerator(_humanPlayer.GlobeNode);

        _sectorManager.CreateSector(sectorGenerator, proceduralGeneratorOptions);

        _propContainerManager.Added += PropContainerManager_Added;
        _propContainerManager.Removed += PropContainerManager_Removed;

        _playerState.TaskSource = _humanActorTaskSource;

        _gameLoop.ActorTaskSources = new[] {
            _humanActorTaskSource,
            _monsterActorTaskSource
        };

        _sectorManager.CurrentSector.ActorExit += SectorOnActorExit;
    }

    private ISectorGeneratorOptions CreateSectorGeneratorOptions(ILocationScheme locationScheme)
    {
        var monsterGeneratorOptions = new MonsterGeneratorOptions
        {
            BotPlayer = _botPlayer
        };

        var proceduralGeneratorOptions = new SectorProceduralGeneratorOptions
        {
            MonsterGeneratorOptions = monsterGeneratorOptions
        };

        if (locationScheme.SectorLevels != null)
        {
            var wellFormedSectorLevel = _personManager.SectorLevel;
            var sectorScheme = locationScheme.SectorLevels[wellFormedSectorLevel];

            monsterGeneratorOptions.RegularMonsterSids = sectorScheme.RegularMonsterSids;
            monsterGeneratorOptions.RareMonsterSids = sectorScheme.RareMonsterSids;
            monsterGeneratorOptions.ChampionMonsterSids = sectorScheme.ChampionMonsterSids;
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

    private void CreateTraderViewModels(IEnumerable<MapNodeVM> nodeViewModels)
    {
        foreach (var trader in _traderManager.Items)
        {
            CreateTraderViewModel(nodeViewModels, trader);
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

    private void CreateTraderViewModel(IEnumerable<MapNodeVM> nodeViewModels, ITrader trader)
    {
        var traderPrefab = GetTraderPrefab(trader);

        var traderViewModel = Instantiate(traderPrefab, transform);

        var traderNodeVm = nodeViewModels.Single(x => x.Node == trader.Node);
        var containerPosition = traderNodeVm.transform.position + new Vector3(0, 0, -1);
        traderViewModel.transform.position = containerPosition;
        traderViewModel.Trader = trader;
        traderViewModel.Selected += TraderViewModel_Selected;

        _traderViewModels.Add(traderViewModel);
    }

    private void TraderViewModel_Selected(object sender, EventArgs e)
    {
        var traderViewModel = sender as TraderViewModel;

        _playerState.HoverViewModel = traderViewModel;

        if (traderViewModel != null)
        {
            _clientCommandExecutor.Push(_showTraderModalCommand);
        }
    }

    private ContainerVm GetContainerPrefab(IPropContainer container)
    {
        if (container is ILootContainer lootContainer)
        {
            return LootPrefab;
        }

        return ChestPrefab;
    }

    private TraderViewModel GetTraderPrefab(ITrader trader)
    {
        return TraderPrefab;
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
        if (!(e.Result is SuccessOpenContainerResult))
        {
            Debug.Log($"Не удалось открыть контейнер {e.Container}.");
        }

        _clientCommandExecutor.Push(_showContainerModalCommand);
    }

    private void SectorOnActorExit(object sender, EventArgs e)
    {
        _interuptCommands = true;
        _commandBlockerService.DropBlockers();
        _playerState.ActiveActor = null;
        _humanActorTaskSource.SwitchActor(null);

        if (_humanPlayer.GlobeNode == null)
        {
            // intro
            _personManager.SectorLevel = 0;
            SceneManager.LoadScene("globe");
            return;
        }

        var currentLocation = _humanPlayer.GlobeNode.Scheme;
        if (currentLocation?.SectorLevels == null)
        {
            _personManager.SectorLevel = 0;
            SceneManager.LoadScene("globe");
            return;
        }

        _personManager.SectorLevel++;
        
        if (_personManager.SectorLevel >= currentLocation.SectorLevels.Length)
        {
            _personManager.SectorLevel = 0;
            SceneManager.LoadScene("globe");
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

            AddResourceToActor(inventory, "med-kit", 1);
            AddResourceToActor(inventory, "water-bottle", 2);
            AddResourceToActor(inventory, "packed-food", 2);
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