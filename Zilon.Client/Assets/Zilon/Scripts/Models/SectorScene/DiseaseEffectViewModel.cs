using Assets.Zilon.Scripts.Services;

using UnityEngine;

using Zenject;

using Zilon.Core.Persons;

public class DiseaseEffectViewModel : MonoBehaviour
{
    [Inject]
    private readonly UiSettingService _uiSettingService;

    public UiElementTooltip UiElementTooltip;

    private IDisease _disease;

    public void Init(IDisease disease)
    {
        if (disease is null)
        {
            throw new System.ArgumentNullException(nameof(disease));
        }

        _disease = disease;

        var currentLanguage = _uiSettingService.CurrentLanguage;

        if (UiElementTooltip != null)
        {
            var effectText = GetEffectText(currentLanguage, _disease);
            UiElementTooltip.text = effectText;
        }
    }

    private static string GetEffectText(Language currentLanguage, IDisease disease)
    {
        return DiseaseLocalizationHelper.GetValueOrDefaultNoname(currentLanguage, disease.Name);
    }
}
