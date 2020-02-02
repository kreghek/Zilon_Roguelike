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

    public void InitNodes(IEnumerable<MapNodeVM> nodeViewModels)
    {
        _nodeViewModels = nodeViewModels.ToArray();
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

        if (_nodeViewModels == null)
        {
            return;
        }

        foreach (var nodeViewModel in _nodeViewModels)
        {
            var fowNode = activeActor.SectorFowData.Nodes.SingleOrDefault(x => x.Node == nodeViewModel.Node);

            var isObserving = fowNode?.State == SectorMapNodeFowState.Observing;

            nodeViewModel.gameObject.SetActive(isObserving);
        }
    }
}
