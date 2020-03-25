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

    [Inject] [NotNull] private readonly HumanPlayer _humanPlayer;
    [Inject] [NotNull] private readonly UiSettingService _uiSettingService;

    public void FixedUpdate()
    {
        var scheme = _humanPlayer.SectorNode.SectorScheme;

        var currentLanguage = _uiSettingService.CurrentLanguage;

        SectorNameText.text = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, scheme.Name);

        Destroy(this);
    }
}
