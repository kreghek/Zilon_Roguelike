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

#pragma warning disable 649
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable NotNullMemberIsNotInitialized
    [NotNull] public MapNodeVM MapNodePrefab;

    [NotNull] public ActorViewModel ActorPrefab;

    [NotNull] public BulletDrive BulletPrefab;

    [NotNull] public HumanoidActorGraphic HumanoidGraphicPrefab;

    [NotNull] public MonoActorGraphic MonoGraphicPrefab;

    [NotNull] public ContainerVm ContainerPrefab;

    [NotNull] [Inject] private IGameLoop _gameLoop;

    [NotNull] [Inject] private ICommandManager _clientCommandExecutor;

    [NotNull] [Inject] private ISectorManager _sectorManager;

    [NotNull] [Inject] private IPlayerState _playerState;

    [NotNull] [Inject] private ISchemeService _schemeService;

    [NotNull] [Inject] private readonly IPropFactory _propFactory;

    [NotNull] [Inject] private readonly HumanPlayer _humanPlayer;

    [NotNull] [Inject] private IActorManager _actorManager;

    [NotNull] [Inject] private IPropContainerManager _propContainerManager;

    [NotNull] [Inject] private IHumanPersonManager _personManager;

    [Inject] private IHumanActorTaskSource _humanActorTaskSource;

    [Inject(Id = "monster")] private readonly IActorTaskSource _monsterActorTaskSource;

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
        _sectorManager.CreateSector();

        _propContainerManager.Added += PropContainerManager_Added;

        _playerState.TaskSource = _humanActorTaskSource;

        _gameLoop.ActorTaskSources = new[] {
            _humanActorTaskSource,
            _monsterActorTaskSource
        };

        _sectorManager.CurrentSector.ActorExit += SectorOnActorExit;
    }

    public void OnDestroy()
    {
        _propContainerManager.Added -= PropContainerManager_Added;
        _sectorManager.CurrentSector.ActorExit -= SectorOnActorExit;
    }

    private void InitPlayerActor(IEnumerable<MapNodeVM> nodeViewModels)
    {
        var personScheme = _schemeService.GetScheme<IPersonScheme>("captain");

        var playerActorStartNode = _sectorManager.CurrentSector.Map.StartNodes.First();
        var playerActorVm = CreateHumanActorVm(_humanPlayer,
            personScheme,
            _actorManager,
            playerActorStartNode,
            nodeViewModels);

        //Лучше централизовать переключение текущего актёра только в playerState
        _playerState.ActiveActor = playerActorVm;
        _humanActorTaskSource.SwitchActor(_playerState.ActiveActor.Actor);
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
            var actorVm = Instantiate(ActorPrefab, transform);
            var actorGraphic = Instantiate(MonoGraphicPrefab, actorVm.transform);
            actorVm.GraphicRoot = actorGraphic;
            actorGraphic.transform.position = new Vector3(0, /*0.2f*/0, 0);

            var graphicController = actorVm.gameObject.AddComponent<MonsterSingleActorGraphicController>();
            graphicController.Actor = monsterActor;
            graphicController.Graphic = actorGraphic;

            var actorNodeVm = nodeViewModels.Single(x => x.Node == monsterActor.Node);
            var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
            actorVm.transform.position = actorPosition;
            actorVm.Actor = monsterActor;

            actorVm.Selected += EnemyActorVm_OnSelected;
            monsterActor.UsedAct += ActorOnUsedAct;
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
        var containerVm = Instantiate(ContainerPrefab, transform);

        var containerNodeVm = nodeViewModels.Single(x => x.Node == container.Node);
        var containerPosition = containerNodeVm.transform.position + new Vector3(0, 0, -1);
        containerVm.transform.position = containerPosition;
        containerVm.Container = container;
        containerVm.Selected += Container_Selected;
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
        SceneManager.LoadScene("combat");
    }

    private void EnemyActorVm_OnSelected(object sender, EventArgs e)
    {
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
        [NotNull] IMapNode startNode,
        [NotNull] IEnumerable<MapNodeVM> nodeVMs)
    {
        if (_personManager.Person == null)
        {
            var inventory = new Inventory();

            var evolutionData = new EvolutionData(_schemeService);

            var defaultActScheme = _schemeService.GetScheme<ITacticalActScheme>(personScheme.DefaultAct);

            var person = new HumanPerson(personScheme, defaultActScheme, evolutionData, inventory);

            _personManager.Person = person;
        }

        var actor = new Actor(_personManager.Person, player, startNode);

        actorManager.Add(actor);

        var actorVm = Instantiate(ActorPrefab, transform);
        var actorGraphic = Instantiate(HumanoidGraphicPrefab, actorVm.transform);
        actorGraphic.transform.position = new Vector3(0, 0.2f, 0);
        actorVm.GraphicRoot = actorGraphic;

        var graphicController = actorVm.gameObject.AddComponent<HumanActorGraphicController>();
        graphicController.Actor = actor;
        graphicController.Graphic = actorGraphic;

        var actorNodeVm = nodeVMs.Single(x => x.Node == actor.Node);
        var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
        actorVm.transform.position = actorPosition;
        actorVm.Actor = actor;

        actorVm.Actor.OpenedContainer += PlayerActorOnOpenedContainer;
        actorVm.Actor.UsedAct += ActorOnUsedAct;

        return actorVm;
    }

    private void AddEquipmentToActor(Inventory inventory, string equipmentSid)
    {
        var equipmentScheme = _schemeService.GetScheme<IPropScheme>(equipmentSid);
        var equipment = _propFactory.CreateEquipment(equipmentScheme);
        inventory.Add(equipment);
    }

    private void ActorOnUsedAct(object sender, UsedActEventArgs e)
    {
        var actor = sender as IActor;
        if (actor == null)
        {
            throw new NotSupportedException("Не поддерживается обработка событий использования действия.");
        }

        var actorHexNode = actor.Node as HexNode;
        var targetHexNode = e.Target.Node as HexNode;

        var distance = actorHexNode.CubeCoords.DistanceTo(targetHexNode.CubeCoords);
        if (distance > 1)
        {
            // Создаём снараяд
            CreateBullet(actor, e.Target);
        }
    }

    private void CreateBullet(IActor actor, IAttackTarget target)
    {
        var viewModels = GetComponentsInChildren<IActorViewModel>();

        var actorViewModel = viewModels.Single(x => x.Actor == actor);
        var targetViewModel = viewModels.Single(x => x.Actor == target);

        var bullet = Instantiate(BulletPrefab, transform);
        bullet.StartObject = ((MonoBehaviour)actorViewModel).gameObject;
        bullet.FinishObject = ((MonoBehaviour)targetViewModel).gameObject;
    }

    private void MapNodeVm_OnSelect(object sender, EventArgs e)
    {
        // указываем намерение двигиться на выбранную точку (узел).

        var nodeVm = sender as MapNodeVM;

        _playerState.HoverViewModel = nodeVm;

        if (nodeVm != null)
        {
            _clientCommandExecutor.Push(_moveCommand);
        }
    }
}