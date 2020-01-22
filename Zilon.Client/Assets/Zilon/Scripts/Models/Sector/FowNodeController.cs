using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Tactics;

[RequireComponent(typeof(MapNodeVM))]
public class FowNodeController : MonoBehaviour
{
    [Inject]
    private readonly ISectorUiState _sectorUiState;

    public MapNodeVM NodeViewModel;

    public GameObject NodeGraphicObject;

    public void Start()
    {
        NodeGraphicObject.SetActive(false);
    }

    public void Update()
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
}
