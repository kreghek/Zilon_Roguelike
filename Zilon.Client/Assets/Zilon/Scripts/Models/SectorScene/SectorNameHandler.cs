using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Players;
using Zilon.Core.Schemes;

public class SectorNameHandler : MonoBehaviour
{
    public Text SectorNameText;

    [Inject] [NotNull] private readonly ISchemeService _schemeService;
    [Inject] [NotNull] private readonly UiSettingService _uiSettingService;

    public void FixedUpdate()
    {
        var currentSchemeSid = _humanPlayer.SectorSid;

        var scheme = _schemeService.GetScheme<ILocationScheme>(currentSchemeSid);

        var currentLanguage = _uiSettingService.CurrentLanguage;

        SectorNameText.text = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, scheme.Name);

        Destroy(this);
    }
}
