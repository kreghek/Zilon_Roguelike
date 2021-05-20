﻿using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

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
    private readonly IPlayer _humanPlayer;

    [NotNull]
    [Inject]
    private readonly ISectorUiState _playerState;

    [NotNull]
    [Inject]
    private readonly DiContainer _container;

    [NotNull]
    [Inject]
    private readonly IPropFactory _propFactory;

    public ActorViewModel InitPlayerActor(IEnumerable<MapNodeVM> nodeViewModels, List<ActorViewModel> ActorViewModels)
    {
        var personScheme = _schemeService.GetScheme<IPersonScheme>("human-person");

        var playerActorStartNode = _humanPlayer.SectorNode.Sector.Map.Regions
            .Single(x => x.IsStart).Nodes
            .First();

        var playerActorViewModel = CreateHumanActorViewModel(
            _humanPlayer.SectorNode.Sector.ActorManager,
            nodeViewModels);

        //Не забывать изменять активного персонажа в источнике команд.
        SetActiveActor(playerActorViewModel);

        ActorViewModels.Add(playerActorViewModel);

        return playerActorViewModel;
    }

    private void SetActiveActor(ActorViewModel playerActorViewModel)
    {
        // Это нужно для UI, чтобы они реагировали на состояние текущего персонажа.
        // И это нужно для команд. Команды берут актуивного актёра из источника команд.
        _playerState.ActiveActor = playerActorViewModel;
    }

    private ActorViewModel CreateHumanActorViewModel([NotNull] IActorManager actorManager,
        [NotNull] IEnumerable<MapNodeVM> nodeVMs)
    {
        var actor = actorManager.Items.Single(x => x.Person == _humanPlayer.MainPerson);

        var actorViewModelObj = _container.InstantiatePrefab(ActorPrefab, transform);
        var actorViewModel = actorViewModelObj.GetComponent<ActorViewModel>();
        actorViewModel.PlayerState = _playerState;
        var actorGraphicObj = _container.InstantiatePrefab(HumanoidGraphicPrefab, actorViewModel.transform);
        var actorGraphic = actorGraphicObj.GetComponent<ActorGraphicBase>();
        actorGraphic.transform.position = new Vector3(0, 0.2f, -0.27f);
        actorViewModel.SetGraphicRoot(actorGraphic);

        var graphicController = actorViewModel.gameObject.AddComponent<HumanActorGraphicController>();
        graphicController.Actor = actor;
        graphicController.Graphic = actorGraphic;

        var actorNodeVm = nodeVMs.Single(x => x.Node == actor.Node);
        var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
        actorViewModel.transform.position = actorPosition;
        actorViewModel.Actor = actor;

        EnsurePlayerPersonHasCampTool(actor);

        return actorViewModel;
    }

    private void EnsurePlayerPersonHasCampTool(IActor actor)
    {
        if (actor.Person.GetModule<IInventoryModule>().CalcActualItems().Any(x => x.Scheme.Sid == "camp-tools"))
        {
            return;
        }

        AddResourceToCurrentPerson("camp-tools");
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
}
