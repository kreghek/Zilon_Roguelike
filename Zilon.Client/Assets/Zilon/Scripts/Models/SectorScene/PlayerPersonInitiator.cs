using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Client.Windows;
using Zilon.Core.Graphs;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

public class PlayerPersonInitiator : MonoBehaviour
{
    [NotNull]
    public ActorViewModel ActorPrefab;

    [NotNull]
    public HumanoidActorGraphic HumanoidGraphicPrefab;

    [NotNull]
    [Inject]
    private readonly ISchemeService _schemeService;

    [NotNull]
    [Inject]
    private readonly ISectorManager _sectorManager;

    [NotNull]
    [Inject]
    private readonly HumanPlayer _humanPlayer;

    [NotNull]
    [Inject]
    private readonly IPerkResolver _perkResolver;

    [NotNull]
    [Inject]
    private readonly ISectorUiState _playerState;

    [NotNull]
    [Inject]
    private readonly IHumanActorTaskSource _humanActorTaskSource;

    [NotNull]
    [Inject]
    private readonly IHumanPersonFactory _humanPersonFactory;

    [NotNull]
    [Inject]
    private readonly ProgressStorageService _progressStorageService;

    [Inject]
    private readonly IPlayerEventLogService _playerEventLogService;

    [NotNull]
    [Inject]
    private readonly DiContainer _container;

    [NotNull]
    [Inject]
    private readonly IPropFactory _propFactory;

    [NotNull]
    [Inject]
    private readonly ISectorModalManager _sectorModalManager;

    public ActorViewModel InitPlayerActor(IEnumerable<MapNodeVM> nodeViewModels, List<ActorViewModel> ActorViewModels)
    {
        var personScheme = _schemeService.GetScheme<IPersonScheme>("human-person");

        var playerActorStartNode = _sectorManager.CurrentSector.Map.Regions
            .Single(x => x.IsStart).Nodes
            .First();

        var playerActorViewModel = CreateHumanActorViewModel(
            _humanPlayer,
            _humanPlayer.SectorNode.Sector.ActorManager,
            _perkResolver,
            playerActorStartNode,
            nodeViewModels);

        //Лучше централизовать переключение текущего актёра только в playerState
        _playerState.ActiveActor = playerActorViewModel;
        _humanActorTaskSource.SwitchActor(_playerState.ActiveActor.Actor);

        ActorViewModels.Add(playerActorViewModel);

        return playerActorViewModel;
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
                var playerPerson = _humanPersonFactory.Create();

                _humanPlayer.MainPerson = playerPerson;

                ShowCreatePersonModal(playerPerson);
            }
        }

        var fowData = new HumanSectorFowData();

        var actor = new Actor(_humanPlayer.MainPerson, player, startNode, perkResolver, fowData);
        _playerEventLogService.Actor = actor;
        _humanPlayer.MainPerson.PlayerEventLogService = _playerEventLogService;

        actorManager.Add(actor);

        var actorViewModelObj = _container.InstantiatePrefab(ActorPrefab, transform);
        var actorViewModel = actorViewModelObj.GetComponent<ActorViewModel>();
        actorViewModel.PlayerState = _playerState;
        var actorGraphic = Instantiate(HumanoidGraphicPrefab, actorViewModel.transform);
        actorGraphic.transform.position = new Vector3(0, 0.2f, -0.27f);
        actorViewModel.SetGraphicRoot(actorGraphic);

        var graphicController = actorViewModel.gameObject.AddComponent<HumanActorGraphicController>();
        graphicController.Actor = actor;
        graphicController.Graphic = actorGraphic;

        var actorNodeVm = nodeVMs.Single(x => x.Node == actor.Node);
        var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
        actorViewModel.transform.position = actorPosition;
        actorViewModel.Actor = actor;

        if (!actor.Person.Inventory.CalcActualItems().Any(x => x.Scheme.Sid == "camp-tools"))
        {
            AddResourceToCurrentPerson("camp-tools");
        }

        return actorViewModel;
    }

    private void ShowCreatePersonModal(HumanPerson playerPerson)
    {
        _sectorModalManager.ShowCreatePersonModal(playerPerson);
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
}
