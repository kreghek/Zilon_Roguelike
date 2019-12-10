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

    private void FixedUpdate()
    {
        if (SectorViewModel.IsInitialized)
        {
            var playerActorStartNode = SectorViewModel.Sector.Map.Regions
            .Single(x => x.IsStart).Nodes
            .First();

            var playerActorViewModel = CreateHumanActorViewModel(
                _humanPlayer,
                SectorViewModel.Sector.ActorManager,
                _perkResolver,
                playerActorStartNode,
                SectorMapViewModel.NodeViewModels);

            ActorsViewModel.ActorViewModels.Add(playerActorViewModel);

            _playerState.ActiveActor = playerActorViewModel;

            Destroy(gameObject);
        }
    }

    private ActorViewModel CreateHumanActorViewModel([NotNull] IPlayer player,
        [NotNull] IActorManager actorManager,
        [NotNull] IPerkResolver perkResolver,
        [NotNull] IGraphNode startNode,
        [NotNull] IEnumerable<MapNodeVM> nodeVMs)
    {
        _humanPlayer.MainPerson = _humanPersonFactory.Create();
        
        var actor = new Actor(_humanPlayer.MainPerson, player, startNode, perkResolver);

        actorManager.Add(actor);

        var actorViewModelObj = _container.InstantiatePrefab(ActorPrefab, TargetObject);
        actorViewModelObj.name = "PlayerActor";
        var actorViewModel = actorViewModelObj.GetComponent<ActorViewModel>();

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

        return actorViewModel;
    }
}
