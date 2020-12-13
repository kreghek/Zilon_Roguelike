using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.Zilon.Scripts.Models.SectorScene;
using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Bot.Players;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.Graphs;
using Zilon.Core.PersonGeneration;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

// ReSharper disable once CheckNamespace
// ReSharper disable once ArrangeTypeModifiers
// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once UnusedMember.Global
public class SectorVM : MonoBehaviour
{
    private readonly List<MapNodeVM> _nodeViewModels;
    private readonly List<StaticObjectViewModel> _staticObjectViewModels;
    private IStaticObjectManager _staticObjectManager;

    private bool _interuptCommands;

#pragma warning disable 649
    // ReSharper disable MemberCanBePrivate.Global
    // ReSharper disable NotNullMemberIsNotInitialized
    [NotNull] public MapNodeVM MapNodePrefab;

    [NotNull] public ActorViewModel ActorPrefab;

    [NotNull] public GunShootTracer GunShootTracer;

    [NotNull] public HumanoidActorGraphic HumanoidGraphicPrefab;

    [NotNull] public MonoActorGraphic MonoGraphicPrefab;

    [NotNull] public HitSfx HitSfx;

    [NotNull] public SceneLoader SceneLoader;

    [NotNull] public ContainerPopup ContainerPopupPrefab;

    [NotNull] public Canvas WindowCanvas;

    [NotNull] public FoundNothingIndicator FoundNothingIndicatorPrefab;

    [NotNull] public FowManager FowManager;

    [NotNull] public SleepShadowManager SleepShadowManager;

    [NotNull] public PlayerPersonInitiator PlayerPersonInitiator;

    [NotNull] [Inject] private readonly DiContainer _container;

    [NotNull] [Inject] private readonly ICommandManager _clientCommandExecutor;

    [NotNull] [Inject] private readonly ISectorUiState _playerState;

    [NotNull] [Inject] private readonly IInventoryState _inventoryState;

    [NotNull] [Inject] private readonly ISchemeService _schemeService;

    [NotNull] [Inject] private readonly IPropFactory _propFactory;

    [NotNull] [Inject] private readonly IPlayer _humanPlayer;

    //TODO Вернуть, когда будет придуман туториал
    //[NotNull] [Inject] private readonly ISectorModalManager _sectorModalManager;

    [NotNull] [Inject] private readonly IScoreManager _scoreManager;

    [NotNull] [Inject] private readonly IPerkResolver _perkResolver;

    [Inject] private readonly ICommandBlockerService _commandBlockerService;

    [Inject] private readonly ILogicStateFactory _logicStateFactory;

    [Inject] private readonly ProgressStorageService _progressStorageService;

    [Inject] private readonly IPersonFactory _humanPersonFactory;

    [Inject] private readonly UiSettingService _uiSettingService;

    [Inject] private readonly IBiomeInitializer _biomeInitializer;

    [Inject]
    private readonly IPlayerEventLogService _playerEventLogService;

    [Inject]
    private readonly StaticObjectViewModelSelector _staticObjectViewModelSelector;

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
    [Inject(Id = "mine-deposit-command")]
    private readonly ICommand _mineDepositCommand;

    [NotNull]
    [Inject]
    private readonly IHumanActorTaskSource<ISectorTaskSourceContext> _humanActorTaskSource;

    public List<ActorViewModel> ActorViewModels { get; }

    public IEnumerable<MapNodeVM> NodeViewModels => _nodeViewModels;

    public SectorVM()
    {
        _nodeViewModels = new List<MapNodeVM>();
        ActorViewModels = new List<ActorViewModel>();
        _staticObjectViewModels = new List<StaticObjectViewModel>();
    }

    public void Start()
    {
        _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    }

    // ReSharper restore NotNullMemberIsNotInitialized
    // ReSharper restore MemberCanBePrivate.Global
#pragma warning restore 649

    // ReSharper disable once UnusedMember.Local
    public void Update()
    {
        if (!_commandBlockerService.HasBlockers)
        {
            try
            {
                ExecuteCommands();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
            }
        }
    }

    public bool CanIntent = true;
    private TaskScheduler _taskScheduler;

    private void ExecuteCommands()
    {
        var command = _clientCommandExecutor.Pop();

        try
        {
            if (command != null)
            {
                command.Execute();

                CanIntent = false;

                if (_interuptCommands)
                {
                    return;
                }

                //if (command is IRepeatableCommand repeatableCommand && CanIntent)
                //{
                //    if (repeatableCommand.CanRepeat())
                //    {
                //        _clientCommandExecutor.Push(repeatableCommand);
                //    }
                //}
            }
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Не удалось выполнить команду {command}.", exception);
        }
    }

    // ReSharper disable once UnusedMember.Local
    public void Awake()
    {
        InitServices();

        var nodeViewModels = InitNodeViewModels();
        _nodeViewModels.AddRange(nodeViewModels);

        var playerActorViewModel = PlayerPersonInitiator.InitPlayerActor(nodeViewModels, ActorViewModels);
        AddPlayerActorEventHandlers(playerActorViewModel);

        CreateMonsterViewModels(nodeViewModels);
        CreateStaticObjectViewModels(nodeViewModels);

        FowManager.InitViewModels(nodeViewModels, ActorViewModels, _staticObjectViewModels);

        //TODO Этот фрагмент нужно заменить следующим:
        // Для сектора/мира добавить события на итерацию.
        // Еще лучше - событие, когда актёр выполняет/начинает выполнять задачу.
        // Не забыть отписываться.
        //_gameLoop.Updated += GameLoop_Updated;

        //TODO Разобраться, почему остаются блоки от перемещения при использовании перехода
        _commandBlockerService.DropBlockers();

        // Изначально канвас отключен.
        // Эта операция нужна, чтобы Start у всяких панелей выполнялся после инициализации
        // таких сервисов, как ISectorUiState. Потому что есть много элементов UI,
        // которые зависят от значения ActiveActor.
        WindowCanvas.gameObject.SetActive(true);

        if (!_gameLoopUpdater.IsStarted)
        {
            _gameLoopUpdater.Start();
        }
    }

    [Inject]
    private readonly GameLoopUpdater _gameLoopUpdater;

    private void AddPlayerActorEventHandlers(ActorViewModel actorViewModel)
    {
        actorViewModel.Selected += HumanActorViewModel_Selected;

        var actor = actorViewModel.Actor;
        actor.OpenedContainer += PlayerActorOnOpenedContainer;
        actor.UsedAct += ActorOnUsedAct;
        actor.Person.GetModule<ISurvivalModule>().Dead += HumanPersonSurvival_Dead;
        actor.UsedProp += Actor_UsedProp;
        actor.DepositMined += Actor_DepositMined;
    }

    private void GameLoop_Updated(object sender, EventArgs e)
    {
        _inventoryState.SelectedProp = null;
    }

    private void StaticObjectManager_Added(object sender, ManagerItemsChangedEventArgs<IStaticObject> e)
    {
        foreach (var staticObject in e.Items)
        {
            CreateStaticObjectViewModel(_nodeViewModels, staticObject);
        }
    }

    private void InitServices()
    {
        var sectorNode = _humanPlayer.SectorNode;

        _staticObjectManager = sectorNode.Sector.StaticObjectManager;

        _staticObjectManager.Added += StaticObjectManager_Added;
        _staticObjectManager.Removed += StaticObjectManager_Removed;

        sectorNode.Sector.TrasitionUsed += Sector_HumanGroupExit;
    }

    private void StaticObjectManager_Removed(object sender, ManagerItemsChangedEventArgs<IStaticObject> e)
    {
        foreach (var container in e.Items)
        {
            var containerViewModel = _staticObjectViewModels.Single(x => x.Container == container);
            _staticObjectViewModels.Remove(containerViewModel);
            Destroy(containerViewModel.gameObject);
        }
    }

    public void OnDestroy()
    {
        _staticObjectManager.Added -= StaticObjectManager_Added;
        _staticObjectManager.Removed -= StaticObjectManager_Removed;

        UnscribeSectorDependentEvents();
    }

    private List<MapNodeVM> InitNodeViewModels()
    {
        var map = _humanPlayer.SectorNode.Sector.Map;
        var nodeVMs = new List<MapNodeVM>();

        foreach (var node in map.Nodes)
        {
            var mapNodeObj = _container.InstantiatePrefab(MapNodePrefab, transform);

            var mapNodeVm = mapNodeObj.GetComponent<MapNodeVM>();

            var hexNode = (HexNode)node;
            var nodeWorldPositionParts = HexHelper.ConvertToWorld(hexNode.OffsetCoords);
            var worldPosition = new Vector3(nodeWorldPositionParts[0], nodeWorldPositionParts[1] / 2);
            mapNodeVm.transform.position = worldPosition;
            mapNodeVm.Node = hexNode;
            mapNodeVm.Neighbors = map.GetNext(node).Cast<HexNode>().ToArray();
            mapNodeVm.LocaltionScheme = _humanPlayer.SectorNode.Sector.Scheme;

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
        var monsters = _humanPlayer.SectorNode.Sector.ActorManager.Items.Where(x => x.Person is MonsterPerson).ToArray();
        foreach (var monsterActor in monsters)
        {
            var actorViewModelObj = _container.InstantiatePrefab(ActorPrefab, transform);
            var actorViewModel = actorViewModelObj.GetComponent<ActorViewModel>();

            var actorGraphic = Instantiate(MonoGraphicPrefab, actorViewModel.transform);
            actorViewModel.SetGraphicRoot(actorGraphic);
            actorGraphic.transform.position = new Vector3(0, /*0.2f*/0, -0.27f);

            var graphicController = actorViewModel.gameObject.AddComponent<MonsterSingleActorGraphicController>();
            graphicController.Actor = monsterActor;
            graphicController.Graphic = actorGraphic;

            var actorNodeVm = nodeViewModels.Single(x => ReferenceEquals(x.Node, monsterActor.Node));
            var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
            actorViewModel.transform.position = actorPosition;
            actorViewModel.Actor = monsterActor;

            actorViewModel.Selected += EnemyActorVm_OnSelected;
            actorViewModel.MouseEnter += EnemyViewModel_MouseEnter;
            monsterActor.UsedAct += ActorOnUsedAct;
            monsterActor.Person.GetModule<ISurvivalModule>().Dead += Monster_Dead;

            var fowController = actorViewModel.gameObject.AddComponent<FowActorController>();
            // Контроллеру тумана войны скармливаем только графику.
            // Потому что на основтой объект акёра завязаны блокировки (на перемещение, например).
            // Если основной объект создаст блокировку и будет отключен,
            // то он не сможет её снять в результате своих Update.
            // Это создаст всеобщую неснимаемую блокировку.
            fowController.Graphic = actorGraphic.gameObject;
            // Передаём коллайдер, чтобы в случае отключения графики скрытого актёра нельзя было выбрать.
            fowController.Collider = actorViewModel.GetComponent<Collider2D>();

            ActorViewModels.Add(actorViewModel);
        }

        var humanActors = _humanPlayer.SectorNode.Sector.ActorManager.Items.Where(x => x.Person is HumanPerson && x.Person != _humanPlayer.MainPerson).ToArray();
        foreach (var actor in humanActors)
        {
            var actorViewModelObj = _container.InstantiatePrefab(ActorPrefab, transform);
            var actorViewModel = actorViewModelObj.GetComponent<ActorViewModel>();
            actorViewModel.PlayerState = _playerState;
            var actorGraphic = Instantiate(HumanoidGraphicPrefab, actorViewModel.transform);
            actorGraphic.transform.position = new Vector3(0, 0.2f, -0.27f);
            actorViewModel.SetGraphicRoot(actorGraphic);

            var graphicController = actorViewModel.gameObject.AddComponent<HumanActorGraphicController>();
            graphicController.Actor = actor;
            graphicController.Graphic = actorGraphic;

            var actorNodeVm = NodeViewModels.Single(x => x.Node == actor.Node);
            var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
            actorViewModel.transform.position = actorPosition;
            actorViewModel.Actor = actor;

            actorViewModel.Selected += EnemyActorVm_OnSelected;
            actorViewModel.MouseEnter += EnemyViewModel_MouseEnter;
            actor.UsedAct += ActorOnUsedAct;
            actor.Person.GetModule<ISurvivalModule>().Dead += Monster_Dead;

            var fowController = actorViewModel.gameObject.AddComponent<FowActorController>();
            // Контроллеру тумана войны скармливаем только графику.
            // Потому что на основтой объект акёра завязаны блокировки (на перемещение, например).
            // Если основной объект создаст блокировку и будет отключен,
            // то он не сможет её снять в результате своих Update.
            // Это создаст всеобщую неснимаемую блокировку.
            fowController.Graphic = actorGraphic.gameObject;
            // Передаём коллайдер, чтобы в случае отключения графики скрытого актёра нельзя было выбрать.
            fowController.Collider = actorViewModel.GetComponent<Collider2D>();

            ActorViewModels.Add(actorViewModel);
        }
    }

    private void Monster_Dead(object sender, EventArgs e)
    {
        // Используем ReferenceEquals, потому что нам нужно сравнить object и ISurvivalData по ссылке.
        // Это делаем, чтобы избежать приведения sender к ISurvivalData.
        var viewModel = ActorViewModels.SingleOrDefault(x => ReferenceEquals(x.Actor.Person.GetModule<ISurvivalModule>(), sender));

        if (viewModel != null)
        {
            ActorViewModels.Remove(viewModel);
        }
    }

    private void CreateStaticObjectViewModels(IEnumerable<MapNodeVM> nodeViewModels)
    {
        foreach (var staticObject in _staticObjectManager.Items)
        {
            CreateStaticObjectViewModel(nodeViewModels, staticObject);
        }
    }

    private void CreateStaticObjectViewModel(IEnumerable<MapNodeVM> nodeViewModels, IStaticObject staticObject)
    {
        var staticObjectPrefab = GetStaticObjectPrefab(staticObject);

        if (staticObjectPrefab is null)
        {
            throw new InvalidOperationException($"Не удаётся выбрать модель представления для объекта {staticObject.Purpose}");
        }

        var staticObjectViewModel = Instantiate(staticObjectPrefab, transform);

        var nodeViewModelUnderStaticObject = nodeViewModels.Single(x => x.Node == staticObject.Node);
        var containerPosition = nodeViewModelUnderStaticObject.transform.position + new Vector3(0, 0, -1);
        staticObjectViewModel.WorldPosition = containerPosition;
        staticObjectViewModel.StaticObject = staticObject;
        staticObjectViewModel.Selected += Container_Selected;
        staticObjectViewModel.MouseEnter += ContainerViewModel_MouseEnter;

        _staticObjectViewModels.Add(staticObjectViewModel);
    }

    private StaticObjectViewModel GetStaticObjectPrefab(IStaticObject staticObject)
    {
        var prefab = _staticObjectViewModelSelector.SelectViewModel(staticObject);
        return prefab;
    }

    private void Container_Selected(object sender, EventArgs e)
    {
        var containerViewModel = sender as StaticObjectViewModel;

        _playerState.HoverViewModel = containerViewModel;
        _playerState.SelectedViewModel = containerViewModel;

        if (containerViewModel == null)
        {
            return;
        }

        if (containerViewModel.Container.HasModule<IPropContainer>() &&
            containerViewModel.Container.GetModule<IPropContainer>().IsActive &&
            _openContainerCommand.CanExecute())
        {
            _clientCommandExecutor.Push(_openContainerCommand);
        }
        else if (containerViewModel.Container.HasModule<IPropDepositModule>() &&
            _mineDepositCommand.CanExecute())
        {
            _clientCommandExecutor.Push(_mineDepositCommand);
        }
        else if (_attackCommand.CanExecute())
        {
            _clientCommandExecutor.Push(_attackCommand);
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

        var propContainer = e.Container.GetModule<IPropContainer>();
        ShowFoundPropsModalOrNotFound(actor, propContainer);
    }

    private void ShowFoundPropsModalOrNotFound(IActor actor, IPropContainer propContainer)
    {
        var props = propContainer.Content.CalcActualItems();
        if (props.Any())
        {
            var containerPopupObj = _container.InstantiatePrefab(ContainerPopupPrefab, WindowCanvas.transform);

            var containerPopup = containerPopupObj.GetComponent<ContainerPopup>();

            var transferMachine = new PropTransferMachine(actor.Person.GetModule<IInventoryModule>(),
                propContainer.Content);
            containerPopup.Init(transferMachine);
        }
        else
        {
            var indicator = Instantiate(FoundNothingIndicatorPrefab, transform);
            indicator.CurrentLanguage = _uiSettingService.CurrentLanguage;

            var actorViewModel = ActorViewModels.SingleOrDefault(x => x.Actor == actor);

            indicator.Init(actorViewModel);
        }
    }

    private void Sector_HumanGroupExit(object sender, TransitionUsedEventArgs e)
    {
        Task.Factory.StartNew(() =>
        {
            try
            {
                HandleSectorTransitionInner();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
            }
        }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler).Wait();
    }

    private void HandleSectorTransitionInner()
    {
        // Персонаж игрока выходит из сектора.
        var actor = _playerState.ActiveActor.Actor;

        // Отписываемся от событий в этом секторе
        UnscribeSectorDependentEvents();

        _interuptCommands = true;
        _commandBlockerService.DropBlockers();

        var activeActor = actor;
        var survivalModule = activeActor.Person.GetModule<ISurvivalModule>();
        survivalModule.Dead -= HumanPersonSurvival_Dead;
        activeActor.UsedAct -= ActorOnUsedAct;

        _playerState.ActiveActor = null;
        _playerState.SelectedViewModel = null;
        _playerState.HoverViewModel = null;

        StartLoadScene();
    }

    private void UnscribeSectorDependentEvents()
    {
        foreach (var sectorNode in _humanPlayer.Globe.SectorNodes)
        {
            foreach (var actor in sectorNode.Sector.ActorManager.Items)
            {
                actor.UsedAct -= ActorOnUsedAct;
                actor.Person.GetModule<ISurvivalModule>().Dead -= HumanPersonSurvival_Dead;

                actor.UsedAct -= ActorOnUsedAct;
                actor.Person.GetModule<ISurvivalModule>().Dead -= Monster_Dead;
            }
        }

        _humanPlayer.SectorNode.Sector.TrasitionUsed -= Sector_HumanGroupExit;
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
        var inventory = _humanPlayer.MainPerson.GetModule<IInventoryModule>();
        AddResource(inventory, resourceSid, count);
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
    public void AddEquipmentToCurrentPerson(string equipmentSid)
    {
        var inventory = _humanPlayer.MainPerson.GetModule<IInventoryModule>();
        AddEquipment(inventory, equipmentSid);
    }

    private void AddResource(IInventoryModule inventory, string resourceSid, int count)
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

    private void AddEquipment(IInventoryModule inventory, string equipmentSid)
    {
        try
        {
            var equipmentScheme = _schemeService.GetScheme<IPropScheme>(equipmentSid);
            var resource = _propFactory.CreateEquipment(equipmentScheme);
            inventory.Add(resource);
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError($"Не найден объект {equipmentSid}");
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
        if (_playerState.ActiveActor == null)
        {
            return;
        }

        var actorViewModel = sender as ActorViewModel;

        _playerState.SelectedViewModel = actorViewModel;

        if (_attackCommand.CanExecute())
        {
            _clientCommandExecutor.Push(_attackCommand);
        }
    }

    private void EnemyViewModel_MouseEnter(object sender, EventArgs e)
    {
        _playerState.HoverViewModel = (IActorViewModel)sender;
    }

    private void Actor_UsedProp(object sender, UsedPropEventArgs e)
    {
        if (e.UsedProp.Scheme.Sid != "camp-tools")
        {
            return;
        }

        SleepShadowManager.StartShadowAnimation();
    }

    private void HumanPersonSurvival_Dead(object sender, EventArgs e)
    {
        _container.InstantiateComponentOnNewGameObject<GameOverEffect>(nameof(GameOverEffect));
        var activeActor = _playerState.ActiveActor.Actor;
        var survivalModule = activeActor.Person.GetModule<ISurvivalModule>();
        survivalModule.Dead -= HumanPersonSurvival_Dead;

        _progressStorageService.Destroy();
    }

    private void ActorOnUsedAct(object sender, UsedActEventArgs e)
    {
        var actor = GetActorFromEventSender(sender);

        var actorHexNode = actor.Node as HexNode;
        var targetHexNode = e.TargetNode as HexNode;

        // Визуализируем удар.
        var actorViewModel = ActorViewModels.Single(x => x.Actor == actor);

        var actEffect = e.TacticalAct.Stats.Effect;
        switch (actEffect)
        {
            case TacticalActEffectType.Damage:
                ProcessDamage(e.TargetNode, e.TacticalAct, actor, actorViewModel);
                break;

            case TacticalActEffectType.Heal:
                ProcessHeal(actorViewModel);
                break;

            case TacticalActEffectType.Undefined:
            default:
                throw new InvalidOperationException($"Неизвестный тип воздействия {actEffect}.");
        }
    }

    private void Actor_DepositMined(object sender, MineDepositEventArgs e)
    {
        var actor = GetActorFromEventSender(sender);

        var depositViewModel = _staticObjectViewModels.Single(x => x.StaticObject == e.Deposit);
        var actorViewModel = ActorViewModels.Single(x => x.Actor == actor);
        actorViewModel.GraphicRoot.ProcessMine(depositViewModel.transform.position);

        var propContainer = e.Deposit.GetModule<IPropContainer>();

        if (e.Result is SuccessMineDepositResult)
        {
            ShowFoundPropsModalOrNotFound(actor, propContainer);
        }
    }

    private static void ProcessHeal(ActorViewModel actorViewModel)
    {
        actorViewModel.GraphicRoot.ProcessHit(actorViewModel.transform.position);
    }

    private void ProcessDamage(IGraphNode targetNode, ITacticalAct tacticalAct, IActor actor, ActorViewModel actorViewModel)
    {
        var targetActorViewModel = ActorViewModels.SingleOrDefault(x => x.Actor.Node == targetNode);
        var targetStaticObjectViewModel = _staticObjectViewModels.SingleOrDefault(x => x.StaticObject.Node == targetNode);
        var canBeHitViewModel = (ICanBeHitSectorObject)targetActorViewModel ?? targetStaticObjectViewModel;
        if (canBeHitViewModel is null)
        {
            return;
        }

        actorViewModel.GraphicRoot.ProcessHit(canBeHitViewModel.Position);

        var sfx = Instantiate(HitSfx, transform);
        canBeHitViewModel.AddHitEffect(sfx);

        // Проверяем, стрелковое оружие или удар ближнего боя
        if (tacticalAct.Stats.Range?.Max > 1)
        {
            sfx.EffectSpriteRenderer.sprite = sfx.ShootSprite;

            // Создаём снараяд
            CreateBullet(actor, targetNode);
        }
    }

    private static IActor GetActorFromEventSender(object sender)
    {
        if (sender is IActor actor)
        {
            return actor;
        }

        throw new NotSupportedException("Не поддерживается обработка событий использования действия.");
    }

    private void CreateBullet(IActor actor, IGraphNode targetNode)
    {
        var actorViewModel = ActorViewModels.Single(x => x.Actor == actor);

        var targetActorViewModel = ActorViewModels.SingleOrDefault(x => x.Actor.Node == targetNode);
        var targetStaticObjectViewModel = _staticObjectViewModels.SingleOrDefault(x => x.StaticObject.Node == targetNode);
        var canBeHitViewModel = (ICanBeHitSectorObject)targetActorViewModel ?? targetStaticObjectViewModel;

        var bulletTracer = Instantiate(GunShootTracer, transform);
        bulletTracer.FromPosition = actorViewModel.transform.position;
        bulletTracer.TargetPosition = canBeHitViewModel.Position;
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