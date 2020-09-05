using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Players;

public class SectorNameHandler : MonoBehaviour
{
    public Text SectorNameText;

    [Inject] [NotNull] private readonly IPlayer _humanPlayer;
    [Inject] [NotNull] private readonly UiSettingService _uiSettingService;

    public void Update()
    {
        var locationScheme = _humanPlayer.SectorNode.Biome.LocationScheme;
        var scheme = _humanPlayer.SectorNode.SectorScheme;

        var currentLanguage = _uiSettingService.CurrentLanguage;

        var locationName = LocalizationHelper.GetValue(currentLanguage, locationScheme.Name);
        var sectorLevelName = LocalizationHelper.GetValue(currentLanguage, scheme.Name);

        SectorNameText.text = $"{locationName} {sectorLevelName}".Trim();

        if (string.IsNullOrWhiteSpace(SectorNameText.text))
        {
            SectorNameText.text = LocalizationHelper.GetUndefined(currentLanguage);
        }

        Destroy(this);
    }
}
