using System.Linq;

using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Players;

public class GlobeWorldVM : MonoBehaviour
{
    public GlobeMapVM Map;
    public Text Text;

    [Inject] private readonly IGlobeManager _globeManager;
    [Inject] private readonly HumanPlayer _player;

    
    void Start()
    {
        if (_globeManager.CurrentGlobe == null)
        {
            _globeManager.GenerateGlobe();
        }

        var currentLocality = _globeManager.CurrentGlobe.Localities.First();
        var currentGlobeCell = currentLocality.Cell;

        var region = _globeManager.GenerateRegion(currentGlobeCell);

        foreach (var gloneRegionNode in region.Nodes)
        {

        }
    }
}