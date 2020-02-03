using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Players;
using Zilon.Core.Schemes;

public class SectorNameHandler : MonoBehaviour
{
    public Text SectorNameText;

    [Inject] [NotNull] private readonly HumanPlayer _humanPlayer;
    [Inject] [NotNull] private readonly ISchemeService _schemeService;

    public void FixedUpdate()
    {
        var currentSchemeSid = _humanPlayer.SectorSid;

        var scheme = _schemeService.GetScheme<ILocationScheme>(currentSchemeSid);
        
        SectorNameText.text = scheme.Name.En;

        Destroy(this);
    }
}
