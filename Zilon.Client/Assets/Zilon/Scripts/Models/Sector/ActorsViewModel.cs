using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Tactics;


public class ActorsViewModel : MonoBehaviour
{
    [NotNull] public SectorMapViewModel MapViewModel;
    [NotNull] public ActorViewModel ActorPrefab;
    [NotNull] public HumanoidActorGraphic HumanoidGraphicPrefab;

    [Inject] private readonly DiContainer _container;

    public ActorsViewModel()
    {
        ActorViewModels = new List<ActorViewModel>();
    }

    public List<ActorViewModel> ActorViewModels { get; }

    public void Init(IActorManager actorManager)
    {
        CreateActorViewModels(actorManager, MapViewModel.NodeViewModels);
    }

    private void CreateActorViewModels(IActorManager _actorManager, IEnumerable<MapNodeVM> nodeViewModels)
    {
        var monsters = _actorManager.Items.ToArray();
        foreach (var actor in monsters)
        {
            var actorViewModelObj = _container.InstantiatePrefab(ActorPrefab, transform);
            var actorViewModel = actorViewModelObj.GetComponent<ActorViewModel>();

            var actorGraphic = Instantiate(HumanoidGraphicPrefab, actorViewModel.transform);
            actorViewModel.GraphicRoot = actorGraphic;
            actorGraphic.transform.position = new Vector3(0, /*0.2f*/0, -0.27f);

            var graphicController = actorViewModel.gameObject.AddComponent<HumanActorGraphicController>();
            graphicController.Actor = actor;
            graphicController.Graphic = actorGraphic;

            var actorNodeVm = nodeViewModels.Single(x => x.Node == actor.Node);
            var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
            actorViewModel.transform.position = actorPosition;
            actorViewModel.Actor = actor;

            ActorViewModels.Add(actorViewModel);
        }
    }
}
