//using Mono.Data.Sqlite;
using System.Linq;
using Assets.Zilon.Scripts.Services;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

using Zenject;
using Zilon.Core.Players;
using Zilon.Core.Schemes;

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
            _globeManager.Generate();
        }

        var currentGlobeCell = _globeManager.CurrentGlobe.Localities.First();


    }
}