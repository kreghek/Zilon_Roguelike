using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Bot.Players;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
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

    [NotNull] [Inject] private readonly IGameLoop _gameLoop;

    [NotNull] [Inject] private readonly ICommandManager _clientCommandExecutor;

    [NotNull] [Inject] private readonly ISectorManager _sectorManager;

    [NotNull] [Inject] private readonly ISectorUiState _playerState;

    [NotNull] [Inject] private readonly IInventoryState _inventoryState;

    [NotNull] [Inject] private readonly ISchemeService _schemeService;

    [NotNull] [Inject] private readonly IPropFactory _propFactory;

    [NotNull] [Inject] private readonly HumanPlayer _humanPlayer;

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
    [Inject(Id = "show-trader-modal-command")]
    private readonly ICommand _showTraderModalCommand;

    [NotNull]
    [Inject(Id = "show-dialog-modal-command")]
    private readonly ICommand _showDialogCommand;

    public List<ActorViewModel> ActorViewModels { get; }

    public IEnumerable<MapNodeVM> NodeViewModels => _nodeViewModels;

    public SectorVM()
    {
        _nodeViewModels = new List<MapNodeVM>();
        ActorViewModels = new List<ActorViewModel>();
        _staticObjectViewModels = new List<StaticObjectViewModel>();
    }

    // ReSharper restore NotNullMemberIsNotInitialized
    // ReSharper restore MemberCanBePrivate.Global
#pragma warning restore 649

    // ReSharper disable once UnusedMember.Local
    public void Update()
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
    public async void Awake()
    {
        await InitServicesAsync();

        var nodeViewModels = InitNodeViewModels();
        _nodeViewModels.AddRange(nodeViewModels);

        var playerActorViewModel = PlayerPersonInitiator.InitPlayerActor(nodeViewModels, ActorViewModels);
        AddPlayerActorEventHandlers(playerActorViewModel);

        CreateMonsterViewModels(nodeViewModels);
        CreateStaticObjectViewModels(nodeViewModels);
        CreateTraderViewModels(nodeViewModels);

        FowManager.InitViewModels(nodeViewModels, ActorViewModels, _staticObjectViewModels);

        _gameLoop.Updated += GameLoop_Updated;

        //TODO Разобраться, почему остаются блоки от перемещения при использовании перехода
        _commandBlockerService.DropBlockers();
    }

    private void AddPlayerActorEventHandlers(ActorViewModel actorViewModel)
    {
        actorViewModel.Selected += HumanActorViewModel_Selected;

        var actor = actorViewModel.Actor;
        actor.OpenedContainer += PlayerActorOnOpenedContainer;
        actor.UsedAct += ActorOnUsedAct;
        actor.Person.Survival.Dead += HumanPersonSurvival_Dead;
        actor.UsedProp += Actor_UsedProp;
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

    private async Task InitServicesAsync()
    {
        var sectorNode = _humanPlayer.SectorNode;

        if (sectorNode == null)
        {
            var introLocationScheme = _schemeService.GetScheme<ILocationScheme>("intro");
            var biom = await _biomeInitializer.InitBiomeAsync(introLocationScheme);
            sectorNode = biom.Sectors.Single(x => x.State == SectorNodeState.SectorMaterialized);
        }
        else if (sectorNode.State == SectorNodeState.SchemeKnown)
        {
            await _biomeInitializer.MaterializeLevelAsync(sectorNode);
        }

        _humanPlayer.BindSectorNode(sectorNode);
        await _sectorManager.CreateSectorAsync();

        sectorNode.Sector.ScoreManager = _scoreManager;

        _staticObjectManager = sectorNode.Sector.StaticObjectManager;

        _staticObjectManager.Added += StaticObjectManager_Added;
        _staticObjectManager.Removed += StaticObjectManager_Removed;

        _playerState.TaskSource = _humanActorTaskSource;

        _gameLoop.ActorTaskSources = new[] {
            _humanActorTaskSource,
            _monsterActorTaskSource
        };

        _sectorManager.CurrentSector.HumanGroupExit += Sector_HumanGroupExit;
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

        _gameLoop.Updated -= GameLoop_Updated;
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
            monsterActor.Person.Survival.Dead += Monster_Dead;

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
        var viewModel = ActorViewModels.SingleOrDefault(x => ReferenceEquals(x.Actor.Person.Survival, sender));

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

    private void CreateTraderViewModels(IEnumerable<MapNodeVM> nodeViewModels)
    {
        var citizens = _humanPlayer.SectorNode.Sector.ActorManager.Items.Where(x => x.Person is CitizenPerson).ToArray();
        foreach (var trader in citizens)
        {
            CreateTraderViewModel(nodeViewModels, trader);
        }
    }

    private void CreateStaticObjectViewModel(IEnumerable<MapNodeVM> nodeViewModels, IStaticObject staticObject)
    {
        var staticObjectPrefab = GetStaticObjectPrefab(staticObject);

        var staticObjectViewModel = Instantiate(staticObjectPrefab, transform);

        var nodeViewModelUnderStaticObject = nodeViewModels.Single(x => x.Node == staticObject.Node);
        var containerPosition = nodeViewModelUnderStaticObject.transform.position + new Vector3(0, 0, -1);
        staticObjectViewModel.WorldPosition = containerPosition;
        staticObjectViewModel.StaticObject = staticObject;
        staticObjectViewModel.Selected += Container_Selected;
        staticObjectViewModel.MouseEnter += ContainerViewModel_MouseEnter;

        _staticObjectViewModels.Add(staticObjectViewModel);
    }

    private void CreateTraderViewModel(IEnumerable<MapNodeVM> nodeViewModels, IActor actor)
    {
        var actorViewModelObj = _container.InstantiatePrefab(ActorPrefab, transform);
        var actorViewModel = actorViewModelObj.GetComponent<ActorViewModel>();

        var actorGraphic = Instantiate(MonoGraphicPrefab, actorViewModel.transform);
        actorViewModel.SetGraphicRoot(actorGraphic);
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

        if (traderViewModel.Actor.Person is CitizenPerson citizen)
        {
            switch (citizen.CitizenType)
            {
                case CitizenType.Unintresting:
                    // Этот тип жителей не интерактивен.
                    break;

                case CitizenType.Trader:
                    if (_showTraderModalCommand.CanExecute())
                    {
                        _clientCommandExecutor.Push(_showTraderModalCommand);
                    }
                    break;

                case CitizenType.QuestGiver:
                    if (_showDialogCommand.CanExecute())
                    {
                        _clientCommandExecutor.Push(_showDialogCommand);
                    }
                    break;
            }


        }
    }

    private StaticObjectViewModel GetStaticObjectPrefab(IStaticObject staticObject)
    {
        var prefab = _staticObjectViewModelSelector.SelectViewModel(staticObject);
        return prefab;
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

        var props = e.Container.GetModule<IPropContainer>().Content.CalcActualItems();
        if (props.Any())
        {
            var containerPopupObj = _container.InstantiatePrefab(ContainerPopupPrefab, WindowCanvas.transform);

            var containerPopup = containerPopupObj.GetComponent<ContainerPopup>();

            var transferMachine = new PropTransferMachine(actor.Person.Inventory,
                e.Container.GetModule<IPropContainer>().Content);
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

    private void Sector_HumanGroupExit(object sender, SectorExitEventArgs e)
    {
        // Персонаж игрока выходит из сектора.
        var actor = _playerState.ActiveActor.Actor;
        _humanPlayer.SectorNode.Sector.ActorManager.Remove(actor);

        // Отписываемся от событий в этом секторе
        UnscribeSectorDependentEvents();

        _interuptCommands = true;
        _commandBlockerService.DropBlockers();
        _humanActorTaskSource.CurrentActor.Person.Survival.Dead -= HumanPersonSurvival_Dead;
        _playerState.ActiveActor = null;
        _playerState.SelectedViewModel = null;
        _playerState.HoverViewModel = null;
        _humanActorTaskSource.SwitchActor(null);

        var nextSectorNode = e.Transition.SectorNode;
        _humanPlayer.BindSectorNode(nextSectorNode);

        StartLoadScene();
    }

    private void UnscribeSectorDependentEvents()
    {
        var monsters = _humanPlayer.SectorNode.Sector.ActorManager.Items.Where(x => x.Person is MonsterPerson).ToArray();
        foreach (var monsterActor in monsters)
        {
            monsterActor.UsedAct -= ActorOnUsedAct;
            monsterActor.Person.Survival.Dead -= Monster_Dead;
        }

        _sectorManager.CurrentSector.HumanGroupExit -= Sector_HumanGroupExit;
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
        var inventory = (Inventory)_humanPlayer.MainPerson.Inventory;
        AddEquipment(inventory, equipmentSid);
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

    private void AddEquipment(Inventory inventory, string equipmentSid)
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

        var actEffect = e.TacticalAct.Stats.Effect;
        switch (actEffect)
        {
            case TacticalActEffectType.Damage:
                ProcessDamage(e.Target, e.TacticalAct, actor, actorViewModel);
                break;

            case TacticalActEffectType.Heal:
                ProcessHeal(actorViewModel);
                break;

            case TacticalActEffectType.Undefined:
            default:
                throw new InvalidOperationException($"Неизвестный тип воздействия {actEffect}.");
        }
    }

    private static void ProcessHeal(ActorViewModel actorViewModel)
    {
        actorViewModel.GraphicRoot.ProcessHit(actorViewModel.transform.position);
    }

    private void ProcessDamage(IAttackTarget target, ITacticalAct tacticalAct, IActor actor, ActorViewModel actorViewModel)
    {
        var targetViewModel = ActorViewModels.SingleOrDefault(x => x.Actor == target);
        if (targetViewModel is null)
        {
            return;
        }

        actorViewModel.GraphicRoot.ProcessHit(targetViewModel.transform.position);

        var sfx = Instantiate(HitSfx, transform);
        targetViewModel.AddHitEffect(sfx);

        // Проверяем, стрелковое оружие или удар ближнего боя
        if (tacticalAct.Stats.Range?.Max > 1)
        {
            sfx.EffectSpriteRenderer.sprite = sfx.ShootSprite;

            // Создаём снараяд
            CreateBullet(actor, target);
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