using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenject;

using Zilon.Bot.Players;
using Zilon.Bot.Players.Strategies;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.Graphs;
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
public class SectorVM : MonoBehaviour
{
    private readonly List<MapNodeVM> _nodeViewModels;
    private readonly List<ContainerVm> _containerViewModels;

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

    [NotNull] public ContainerVm TrashPrefab;

    [NotNull] public HitSfx HitSfx;

    [NotNull] public SceneLoader SceneLoader;

    [NotNull] public ContainerPopup ContainerPopupPrefab;

    [NotNull] public Canvas WindowCanvas;

    [NotNull] public FoundNothingIndicator FoundNothingIndicatorPrefab;

    [NotNull] [Inject] private readonly DiContainer _container;

    [NotNull] [Inject] private readonly ICommandManager<SectorCommandContext> _clientCommandExecutor;

    [NotNull] [Inject] private readonly ISectorManager _sectorManager;

    [NotNull] [Inject] private readonly ISectorUiState _playerState;

    [NotNull] [Inject] private readonly IInventoryState _inventoryState;

    [NotNull] [Inject] private readonly ISchemeService _schemeService;

    [NotNull] [Inject] private readonly IPropFactory _propFactory;

    [NotNull] [Inject] private readonly HumanPlayer _humanPlayer;

    [NotNull] [Inject] private readonly IActorManager _actorManager;

    [NotNull] [Inject] private readonly IPropContainerManager _propContainerManager;

    //TODO Вернуть, когда будет придуман туториал
    //[NotNull] [Inject] private readonly ISectorModalManager _sectorModalManager;

    [NotNull] [Inject] private readonly IScoreManager _scoreManager;

    [NotNull] [Inject] private readonly IPerkResolver _perkResolver;

    [Inject] private readonly IHumanActorTaskSource _humanActorTaskSource;

    [Inject(Id = "monster")] private readonly IActorTaskSource _monsterActorTaskSource;

    [Inject] private readonly ICommandBlockerService _commandBlockerService;

    [Inject] private readonly ILogicStateFactory _logicStateFactory;

    [Inject] private readonly ProgressStorageService _progressStorageService;

    [Inject] private readonly IHumanPersonFactory _humanPersonFactory;

    [Inject] private readonly ScoreStorage _scoreStorage;

    [NotNull]
    [Inject(Id = "move-command")]
    private readonly ICommand<SectorCommandContext> _moveCommand;

    [NotNull]
    [Inject(Id = "attack-command")]
    private readonly ICommand<SectorCommandContext> _attackCommand;

    [NotNull]
    [Inject(Id = "open-container-command")]
    private readonly ICommand<SectorCommandContext> _openContainerCommand;

    [NotNull]
    [Inject(Id = "show-trader-modal-command")]
    private readonly ICommand<SectorCommandContext> _showTraderModalCommand;

    [NotNull]
    [Inject(Id = "show-dialog-modal-command")]
    private readonly ICommand<SectorCommandContext> _showDialogCommand;

    public List<ActorViewModel> ActorViewModels { get; }

    public IEnumerable<MapNodeVM> NodeViewModels => _nodeViewModels;

    public SectorVM()
    {
        _nodeViewModels = new List<MapNodeVM>();
        ActorViewModels = new List<ActorViewModel>();
        _containerViewModels = new List<ContainerVm>();
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
        //var command = _clientCommandExecutor.Pop();

        //try
        //{
        //    if (command != null)
        //    {
        //        command.Execute();

        //        if (_interuptCommands)
        //        {
        //            return;
        //        }

        //        if (command is IRepeatableCommand repeatableCommand)
        //        {
        //            if (repeatableCommand.CanRepeat())
        //            {
        //                _clientCommandExecutor.Push(repeatableCommand);
        //            }
        //        }
        //    }
        //}
        //catch (Exception exception)
        //{
        //    throw new InvalidOperationException($"Не удалось выполнить команду {command}.", exception);
        //}
    }

    // ReSharper disable once UnusedMember.Local
    public async void Awake()
    {
        await InitServicesAsync();

        var nodeViewModels = InitNodeViewModels();
        _nodeViewModels.AddRange(nodeViewModels);

        InitPlayerActor(nodeViewModels);
        CreateMonsterViewModels(nodeViewModels);
        CreateContainerViewModels(nodeViewModels);
        CreateTraderViewModels(nodeViewModels);

        //_gameLoop.Updated += GameLoop_Updated;

        //TODO Разобраться, почему остаются блоки от перемещения при использовании перехода
        _commandBlockerService.DropBlockers();
    }

    private void GameLoop_Updated(object sender, EventArgs e)
    {
        _inventoryState.SelectedProp = null;
    }

    private void PropContainerManager_Added(object sender, ManagerItemsChangedEventArgs<IPropContainer> e)
    {
        foreach (var container in e.Items)
        {
            CreateContainerViewModel(_nodeViewModels, container);
        }
    }

    private async Task InitServicesAsync()
    {
        await _sectorManager.CreateSectorAsync();

        _sectorManager.CurrentSector.ScoreManager = _scoreManager;

        _propContainerManager.Added += PropContainerManager_Added;
        _propContainerManager.Removed += PropContainerManager_Removed;

        _playerState.TaskSource = _humanActorTaskSource;

        _sectorManager.CurrentSector.HumanGroupExit += Sector_HumanGroupExit;
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
        _sectorManager.CurrentSector.HumanGroupExit -= Sector_HumanGroupExit;
    }

    private void InitPlayerActor(IEnumerable<MapNodeVM> nodeViewModels)
    {
        var personScheme = _schemeService.GetScheme<IPersonScheme>("human-person");

        var playerActorStartNode = _sectorManager.CurrentSector.Map.Regions
            .Single(x => x.IsStart).Nodes
            .First();

        var playerActorViewModel = CreateHumanActorViewModel(_humanPlayer,
            _actorManager,
            _perkResolver,
            playerActorStartNode,
            nodeViewModels);

        //Лучше централизовать переключение текущего актёра только в playerState
        _playerState.ActiveActor = playerActorViewModel;
        _humanActorTaskSource.SwitchActor(_playerState.ActiveActor.Actor);

        ActorViewModels.Add(playerActorViewModel);
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
            mapNodeVm.LocaltionScheme = _sectorManager.CurrentSector.Scheme;

            if (map.Transitions.ContainsKey(node))
            {
                mapNodeVm.IsExit = true;
            }

            mapNodeVm.OnSelect += MapNodeVm_OnSelect;
            mapNodeVm.MouseEnter += MapNodeVm_MouseEnter;

            nodeVMs.Add(mapNodeVm);
        }

        return nodeVMs;
    }

    private void MapNodeVm_MouseEnter(object sender, EventArgs e)
    {
        var blocked = _commandBlockerService.HasBlockers;
        if (!blocked)
        {
            _playerState.HoverViewModel = (IMapNodeViewModel)sender;
        }
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
            actorGraphic.transform.position = new Vector3(0, /*0.2f*/0, -0.27f);

            var graphicController = actorViewModel.gameObject.AddComponent<MonsterSingleActorGraphicController>();
            graphicController.Actor = monsterActor;
            graphicController.Graphic = actorGraphic;

            var actorNodeVm = nodeViewModels.Single(x => x.Node == monsterActor.Node);
            var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
            actorViewModel.transform.position = actorPosition;
            actorViewModel.Actor = monsterActor;

            actorViewModel.Selected += EnemyActorVm_OnSelected;
            actorViewModel.MouseEnter += EnemyViewModel_MouseEnter;
            monsterActor.UsedAct += ActorOnUsedAct;

            ActorViewModels.Add(actorViewModel);
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
        var citizens = _actorManager.Items.Where(x => x.Person is CitizenPerson).ToArray();
        foreach (var trader in citizens)
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
        containerViewModel.MouseEnter += ContainerViewModel_MouseEnter;

        _containerViewModels.Add(containerViewModel);
    }

    private void CreateTraderViewModel(IEnumerable<MapNodeVM> nodeViewModels, IActor actor)
    {
        var actorViewModelObj = _container.InstantiatePrefab(ActorPrefab, transform);
        var actorViewModel = actorViewModelObj.GetComponent<ActorViewModel>();

        var actorGraphic = Instantiate(MonoGraphicPrefab, actorViewModel.transform);
        actorViewModel.GraphicRoot = actorGraphic;
        actorGraphic.transform.position = new Vector3(0, /*0.2f*/0, -0.27f);

        var graphicController = actorViewModel.gameObject.AddComponent<MonsterSingleActorGraphicController>();
        graphicController.Actor = actor;
        graphicController.Graphic = actorGraphic;

        var actorNodeVm = nodeViewModels.Single(x => x.Node == actor.Node);
        var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
        actorViewModel.transform.position = actorPosition;
        actorViewModel.Actor = actor;

        actorViewModel.Selected += TraderViewModel_Selected;
        
        ActorViewModels.Add(actorViewModel);
    }

    private void TraderViewModel_Selected(object sender, EventArgs e)
    {
        var traderViewModel = sender as ActorViewModel;

        _playerState.SelectedViewModel = traderViewModel;

        var citizen = traderViewModel.Actor.Person as CitizenPerson;
        if (citizen != null)
        {
            switch (citizen.CitizenType)
            {
                case CitizenType.Unintresting:
                    // Этот тип жителей не интерактивен.
                    break;

                //case CitizenType.Trader:
                //    if (_showTraderModalCommand.CanExecute())
                //    {
                //        _clientCommandExecutor.Push(_showTraderModalCommand);
                //    }
                //    break;

                //case CitizenType.QuestGiver:
                //    if (_showDialogCommand.CanExecute())
                //    {
                //        _clientCommandExecutor.Push(_showDialogCommand);
                //    }
                //    break;
            }

            
        }
    }

    private ContainerVm GetContainerPrefab(IPropContainer container)
    {
        if (container is ILootContainer)
        {
            return LootPrefab;
        }

        if (container.Purpose == PropContainerPurpose.Treasures)
        {
            return ChestPrefab;
        }

        return TrashPrefab;
    }

    private void Container_Selected(object sender, EventArgs e)
    {
        var containerViewModel = sender as ContainerVm;

        _playerState.HoverViewModel = containerViewModel;
        _playerState.SelectedViewModel = containerViewModel;

        if (containerViewModel != null)
        {
            _clientCommandExecutor.Push(_openContainerCommand);
        }
    }

    private void ContainerViewModel_MouseEnter(object sender, EventArgs e)
    {
        var containerViewModel = sender as ContainerVm;

        _playerState.HoverViewModel = containerViewModel;
    }

    private void PlayerActorOnOpenedContainer(object sender, OpenContainerEventArgs e)
    {
        var actor = sender as IActor;

        if (!(e.Result is SuccessOpenContainerResult))
        {
            Debug.Log($"Не удалось открыть контейнер {e.Container}.");
        }

        var props = e.Container.Content.CalcActualItems();
        if (props.Any())
        {
            var containerPopupObj = _container.InstantiatePrefab(ContainerPopupPrefab, WindowCanvas.transform);

            var containerPopup = containerPopupObj.GetComponent<ContainerPopup>();

            var transferMachine = new PropTransferMachine(actor.Person.Inventory, e.Container.Content);
            containerPopup.Init(transferMachine);
        }
        else
        {
            var indicator = Instantiate<FoundNothingIndicator>(FoundNothingIndicatorPrefab, transform);

            var actorViewModel = ActorViewModels.SingleOrDefault(x=>x.Actor == actor);

            indicator.Init(actorViewModel);
        }
    }

    private void Sector_HumanGroupExit(object sender, SectorExitEventArgs e)
    {
        _interuptCommands = true;
        _commandBlockerService.DropBlockers();
        _humanActorTaskSource.CurrentActor.Person.Survival.Dead -= HumanPersonSurvival_Dead;
        _playerState.ActiveActor = null;
        _playerState.SelectedViewModel = null;
        _playerState.HoverViewModel = null;
        _humanActorTaskSource.SwitchActor(null);

        if (_humanPlayer.GlobeNode == null)
        {
            // intro

            if (e.Transition.SectorSid == null)
            {
                AddResourceToCurrentPerson("history-book");
                _humanPlayer.SectorSid = null;
                SceneManager.LoadScene("globe");
                SaveGameProgress();
                return;
            }
            else
            {
                _humanPlayer.SectorSid = e.Transition.SectorSid;
                StartLoadScene();
                return;
            }
        }

        var currentLocation = _humanPlayer.GlobeNode.Scheme;
        if (currentLocation?.SectorLevels == null)
        {
            _humanPlayer.SectorSid = null;
            SceneManager.LoadScene("globe");
            SaveGameProgress();
            return;
        }

        if (e.Transition.SectorSid == null)
        {
            _humanPlayer.SectorSid = null;
            SceneManager.LoadScene("globe");
            SaveGameProgress();
        }
        else
        {
            _humanPlayer.SectorSid = e.Transition.SectorSid;
            StartLoadScene();
        }
    }

    //TODO Вынести в отдельный сервис. Этот функционал может обрасти логикой и может быть использован в ботах и тестах.
    /// <summary>
    /// Добавляет ресурс созданному персонажу игрока.
    /// </summary>
    /// <param name="resourceSid"> Идентификатор предмета. </param>
    /// <param name="count"> Количество ресурсво. </param>
    /// <remarks>
    /// Используется, чтобы добавить персонажу игрока книгу истории, когда он
    /// выходит из стартовой локации, и начинается создание мира.
    /// </remarks>
    public void AddResourceToCurrentPerson(string resourceSid, int count = 1)
    {
        var inventory = (Inventory)_humanPlayer.MainPerson.Inventory;
        AddResource(inventory, resourceSid, count);
    }

    private void AddResource(Inventory inventory, string resourceSid, int count)
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

    private void HumanActorViewModel_Selected(object sender, EventArgs e)
    {
        var actorViewModel = sender as ActorViewModel;

        _playerState.SelectedViewModel = actorViewModel;

        if (actorViewModel != null)
        {
            _clientCommandExecutor.Push(_attackCommand);
        }
    }

    private void EnemyActorVm_OnSelected(object sender, EventArgs e)
    {
        //if (_playerState.ActiveActor == null)
        //{
        //    return;
        //}

        //var actorViewModel = sender as ActorViewModel;

        //_playerState.SelectedViewModel = actorViewModel;

        //if (_attackCommand.CanExecute())
        //{
        //    _clientCommandExecutor.Push(_attackCommand);
        //}
    }

    private void EnemyViewModel_MouseEnter(object sender, EventArgs e)
    {
        _playerState.HoverViewModel = (IActorViewModel)sender;
    }

    private ActorViewModel CreateHumanActorViewModel([NotNull] IPlayer player,
        [NotNull] IActorManager actorManager,
        [NotNull] IPerkResolver perkResolver,
        [NotNull] IGraphNode startNode,
        [NotNull] IEnumerable<MapNodeVM> nodeVMs)
    {
        if (_humanPlayer.MainPerson == null)
        {
            if (!_progressStorageService.LoadPerson())
            {
                _humanPlayer.MainPerson = _humanPersonFactory.Create();
            }
        }

        var actor = new Actor(_humanPlayer.MainPerson, player, startNode, perkResolver);

        actorManager.Add(actor);

        var actorViewModelObj = _container.InstantiatePrefab(ActorPrefab, transform);
        var actorViewModel = actorViewModelObj.GetComponent<ActorViewModel>();
        actorViewModel.PlayerState = _playerState;
        var actorGraphic = Instantiate(HumanoidGraphicPrefab, actorViewModel.transform);
        actorGraphic.transform.position = new Vector3(0, 0.2f, -0.27f);
        actorViewModel.GraphicRoot = actorGraphic;

        var graphicController = actorViewModel.gameObject.AddComponent<HumanActorGraphicController>();
        graphicController.Actor = actor;
        graphicController.Graphic = actorGraphic;

        var actorNodeVm = nodeVMs.Single(x => x.Node == actor.Node);
        var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
        actorViewModel.transform.position = actorPosition;
        actorViewModel.Actor = actor;
        actorViewModel.Selected += HumanActorViewModel_Selected;

        actor.OpenedContainer += PlayerActorOnOpenedContainer;
        actor.UsedAct += ActorOnUsedAct;
        actor.Person.Survival.Dead += HumanPersonSurvival_Dead;

        return actorViewModel;
    }

    private void HumanPersonSurvival_Dead(object sender, EventArgs e)
    {
        var scores = _scoreManager.Scores;

        try
        {
            _scoreStorage.AppendScores("test", scores);
        }
        catch (Exception exception)
        {
            Debug.LogError("Не удалось выполнить запись результатов в БД\n" + exception.ToString());
        }

        _container.InstantiateComponentOnNewGameObject<GameOverEffect>(nameof(GameOverEffect));
        _humanActorTaskSource.CurrentActor.Person.Survival.Dead -= HumanPersonSurvival_Dead;

        _progressStorageService.Destroy();
    }

    private void ActorOnUsedAct(object sender, UsedActEventArgs e)
    {
        var actor = GetActor(sender);

        var actorHexNode = actor.Node as HexNode;
        var targetHexNode = e.Target.Node as HexNode;

        // Визуализируем удар.
        var actorViewModel = ActorViewModels.Single(x => x.Actor == actor);

        if (e.TacticalAct.Stats.Effect == TacticalActEffectType.Damage)
        {
            var targetViewModel = ActorViewModels.Single(x => x.Actor == e.Target);

            actorViewModel.GraphicRoot.ProcessHit(targetViewModel.transform.position);

            var sfx = Instantiate(HitSfx, transform);
            targetViewModel.AddHitEffect(sfx);

            // Проверяем, стрелковое оружие или удар ближнего боя
            if (e.TacticalAct.Stats.Range?.Max > 1)
            {
                sfx.EffectSpriteRenderer.sprite = sfx.ShootSprite;

                // Создаём снараяд
                CreateBullet(actor, e.Target);
            }
        }
        else if (e.TacticalAct.Stats.Effect == TacticalActEffectType.Heal)
        {
            actorViewModel.GraphicRoot.ProcessHit(actorViewModel.transform.position);
            Debug.Log($"{actor} healed youself");
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
        var actorViewModel = ActorViewModels.Single(x => x.Actor == actor);
        var targetViewModel = ActorViewModels.Single(x => x.Actor == target);

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

        _playerState.SelectedViewModel = nodeVm;
        _playerState.HoverViewModel = nodeVm;

        if (nodeVm != null)
        {
            _clientCommandExecutor.Push(_moveCommand);
        }
    }

    private void StartLoadScene()
    {
        SceneLoader.gameObject.SetActive(true);
    }

    private void SaveGameProgress()
    {
        _progressStorageService.Save();
    }
}