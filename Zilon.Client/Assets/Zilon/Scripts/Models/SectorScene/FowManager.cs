using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;

public class FowManager : MonoBehaviour
{
    private const float UPDATE_FOW_DELAY = 0.3f;
    private float _fowUpdateCounter;

    [Inject]
    private readonly ISectorUiState _sectorUiState;

    [Inject]
    private readonly IPlayer _player;

    private MapNodeVM[] _nodeViewModels;
    private IList<ActorViewModel> _actorViewModels;
    private IList<StaticObjectViewModel> _staticObjectViewModels;

    public void Update()
    {
        if (_fowUpdateCounter >= UPDATE_FOW_DELAY)
        {
            UpdateFowState();
        }
        else
        {
            _fowUpdateCounter += Time.deltaTime;
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

    private void UpdateFowState()
    {
        var activeActor = _sectorUiState?.ActiveActor?.Actor;
        if (activeActor == null)
        {
            return;
        }

        var fowData = activeActor.Person.GetModuleSafe<IFowData>();
        Debug.Log(fowData);
        if (fowData != null)
        {
            var sector = _player.Globe.SectorNodes.SingleOrDefault(node => node.Sector.ActorManager.Items.Any(actor => actor.Person == _player.MainPerson))?.Sector;
            if (sector != null)
            {
                Debug.Log(sector);
                var sectorFowData = fowData.GetSectorFowData(sector);
                ProcessNodeFow(sectorFowData);
                ProcessActorFow(sectorFowData);
                ProcessContainerFow(sectorFowData);
            }
        }
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
