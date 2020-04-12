using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Graphs;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

public class PlayerPersonActivator : MonoBehaviour
{
    [NotNull] [Inject] private readonly HumanPlayer _humanPlayer;
    [Inject] private readonly IHumanActorTaskSource _humanActorTaskSource;
    [Inject] private readonly IHumanPersonFactory _humanPersonFactory;
    [NotNull] [Inject] private readonly DiContainer _container;
    [NotNull] [Inject] private readonly IPerkResolver _perkResolver;
    [NotNull] [Inject] private readonly ISectorUiState _playerState;

    [NotNull] public ActorViewModel ActorPrefab;
    [NotNull] public HumanoidActorGraphic HumanoidGraphicPrefab;
    public SectorViewModel SectorViewModel;
    public ActorsViewModel ActorsViewModel;
    public SectorMapViewModel SectorMapViewModel;
    public Transform TargetObject;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", 
        Justification = "Неявно вызывается Unity.")]
    private void FixedUpdate()
    {
        if (SectorViewModel.IsInitialized)
        {
            _humanPlayer.MainPerson = _humanPersonFactory.Create();

            var playerActorViewModel = SelectNodeAndCreateHumanPersonViewModelFromMainPerson();

            ActorsViewModel.ActorViewModels.Add(playerActorViewModel);

            _playerState.ActiveActor = playerActorViewModel;
            _humanActorTaskSource.SwitchActor(playerActorViewModel.Actor);

            Destroy(gameObject);
        }
    }

    private ActorViewModel SelectNodeAndCreateHumanPersonViewModelFromMainPerson()
    {
        var playerActorStartNode = SectorViewModel.Sector.Map.Regions
                    .Single(x => x.IsStart).Nodes
                    .First();

        var playerActorViewModel = CreateHumanActorViewModelFromMainPerson(
            SectorViewModel.Sector.ActorManager,
            _perkResolver,
            playerActorStartNode,
            SectorMapViewModel.NodeViewModels);

        return playerActorViewModel;
    }

    private ActorViewModel CreateHumanActorViewModelFromMainPerson(
        [NotNull] IActorManager actorManager,
        [NotNull] IPerkResolver perkResolver,
        [NotNull] IGraphNode startNode,
        [NotNull] IEnumerable<MapNodeVM> nodeVMs)
    {
        var fowData = new HumanSectorFowData();
        var actor = new Actor(_humanPlayer.MainPerson, _humanPlayer, startNode, perkResolver, fowData);

        FowHelper.UpdateFowData(actor.SectorFowData, SectorViewModel.Sector.Map, startNode, 5);

        actorManager.Add(actor);

        var actorViewModelObj = _container.InstantiatePrefab(ActorPrefab, TargetObject);
        // Задаём имя объекта для отладки через редактор.
        // Чтобы визуально можно было проще найти вью-модель персонажа игрока в секторе.
        actorViewModelObj.name = "PlayerActor";
        var actorViewModel = actorViewModelObj.GetComponent<ActorViewModel>();

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

        return actorViewModel;
    }
}
