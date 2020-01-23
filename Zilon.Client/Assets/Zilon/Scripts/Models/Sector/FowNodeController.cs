using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Tactics;

[RequireComponent(typeof(MapNodeVM))]
public class FowNodeController : MonoBehaviour
{
    private const float UPDATE_FOW_DELAY = 0.3f;
    private float _fowUpdateCounter;

    [Inject]
    private readonly ISectorUiState _sectorUiState;

    public MapNodeVM NodeViewModel;

    public GameObject NodeGraphicObject;

    public void Start()
    {
        PrepareSlowUpdate();
        NodeGraphicObject.SetActive(false);
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

        var fowNode = activeActor.SectorFowData.Nodes.SingleOrDefault(x => x.Node == NodeViewModel.Node);
        var isObserving = fowNode?.State == SectorMapNodeFowState.Observing;

        NodeGraphicObject.SetActive(isObserving);
    }

    void FixedUpdate()
    {
        if (Time.fixedTime >= _fowUpdateCounter)
        {
            UpdateFowState();

            _fowUpdateCounter = Time.fixedTime + UPDATE_FOW_DELAY;
        }
    }
}
