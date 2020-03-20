using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

[RequireComponent(typeof(MapNodeVM))]
public class FowNodeController : MonoBehaviour
{
    private const float UPDATE_FOW_DELAY = 0.3f;
    private float _fowUpdateCounter;

    [Inject]
    private readonly ISectorUiState _sectorUiState;

    public MapNodeVM NodeViewModel;

    public GameObject FowObject;
    private SectorMapFowNode _fowNode;

    public ISectorMap SectorMap { get; set; }

    public void Start()
    {
        PrepareSlowUpdate();
        FowObject.SetActive(true);
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

        // Не обрабатываем узлы, которые далеко от активного актёра. Их всё равно не видно.
        var distance = SectorMap.DistanceBetween(activeActor.Node, NodeViewModel.Node);
        if (distance > 10)
        {
            return;
        }

        if (_fowNode == null)
        {
            var fowNode = activeActor.SectorFowData.Nodes.SingleOrDefault(x => x.Node == NodeViewModel.Node);

            _fowNode = fowNode;
        }

        UpdateVisibilityUsingFowState();
    }

    private void UpdateVisibilityUsingFowState()
    {
        if (_fowNode == null)
        {
            return;
        }

        var isObserving = _fowNode.State == SectorMapNodeFowState.Observing;

        FowObject.SetActive(!isObserving);
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
