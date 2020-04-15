using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Tactics;

public class FowManager : MonoBehaviour
{
    private const float UPDATE_FOW_DELAY = 0.3f;
    private float _fowUpdateCounter;

    [Inject]
    private readonly ISectorUiState _sectorUiState;

    private MapNodeVM[] _nodeViewModels;
    private IList<ActorViewModel> _actorViewModels;
    private IList<StaticObjectViewModel> _staticObjectViewModels;

    public void Start()
    {
        PrepareSlowUpdate();
    }

    public void FixedUpdate()
    {
        if (Time.fixedTime >= _fowUpdateCounter)
        {
            UpdateFowState();

            _fowUpdateCounter = Time.fixedTime + UPDATE_FOW_DELAY;
        }
    }

    public void InitViewModels(IEnumerable<MapNodeVM> nodeViewModels, IList<ActorViewModel> actorViewModels, IList<StaticObjectViewModel> staticObjectViewModels)
    {
        if (nodeViewModels is null)
        {
            throw new System.ArgumentNullException(nameof(nodeViewModels));
        }

        if (actorViewModels is null)
        {
            throw new System.ArgumentNullException(nameof(actorViewModels));
        }

        if (staticObjectViewModels is null)
        {
            throw new System.ArgumentNullException(nameof(staticObjectViewModels));
        }

        _nodeViewModels = nodeViewModels.ToArray();

        _actorViewModels = actorViewModels;

        _staticObjectViewModels = staticObjectViewModels;
    }

    private void PrepareSlowUpdate()
    {
        _fowUpdateCounter = Time.fixedTime + UPDATE_FOW_DELAY;
    }

    private void UpdateFowState()
    {
        var activeActor = _sectorUiState?.ActiveActor?.Actor;
        if (activeActor == null)
        {
            return;
        }

        ProcessNodeFow(activeActor.SectorFowData);
        ProcessActorFow(activeActor.SectorFowData);
        ProcessContainerFow(activeActor.SectorFowData);
    }

    private void ProcessNodeFow(ISectorFowData sectorFowData)
    {
        if (_nodeViewModels == null)
        {
            return;
        }

        foreach (var nodeViewModel in _nodeViewModels)
        {
            var fowNode = sectorFowData.Nodes.SingleOrDefault(x => x.Node == nodeViewModel.Node);

            var fowState = (fowNode?.State).GetValueOrDefault(SectorMapNodeFowState.TerraIncognita);

            var fowController = nodeViewModel.GetComponent<FowNodeController>();

            if (fowController != null)
            {
                fowController.ChangeState(fowState);
            }
        }
    }

    private void ProcessActorFow(ISectorFowData sectorFowData)
    {
        if (_nodeViewModels == null)
        {
            return;
        }

        foreach (var actorViewModel in _actorViewModels.ToArray())
        {
            var fowNode = sectorFowData.Nodes.SingleOrDefault(x => x.Node == actorViewModel.Actor.Node);

            var fowState = (fowNode?.State).GetValueOrDefault(SectorMapNodeFowState.TerraIncognita);

            var fowController = actorViewModel.GetComponent<FowActorController>();

            if (fowController != null)
            {
                fowController.ChangeState(fowState);
            }
        }
    }

    private void ProcessContainerFow(ISectorFowData sectorFowData)
    {
        if (_staticObjectViewModels == null)
        {
            return;
        }

        foreach (var containerViewModel in _staticObjectViewModels.ToArray())
        {
            var fowNode = sectorFowData.Nodes.SingleOrDefault(x => x.Node == containerViewModel.Container.Node);

            var fowState = (fowNode?.State).GetValueOrDefault(SectorMapNodeFowState.TerraIncognita);

            var fowController = containerViewModel.GetComponent<FowContainerController>();

            if (fowController != null)
            {
                fowController.ChangeState(fowState);
            }
        }
    }
}
