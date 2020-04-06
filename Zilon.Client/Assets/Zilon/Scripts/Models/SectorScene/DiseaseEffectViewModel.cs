using System.Collections.Generic;

using Assets.Zilon.Scripts.Services;

using UnityEngine;

using Zenject;

using Zilon.Core.Persons.Survival;

public class DiseaseEffectViewModel : MonoBehaviour
{
    [Inject]
    private readonly UiSettingService _uiSettingService;

    public UiElementTooltip UiElementTooltip;

    private DiseaseSymptomEffect _diseaseSymptomEffect;

    public void Init(DiseaseSymptomEffect diseaseSymptomEffect)
    {
        if (diseaseSymptomEffect is null)
        {
            throw new System.ArgumentNullException(nameof(diseaseSymptomEffect));
        }

        _diseaseSymptomEffect = diseaseSymptomEffect;

        var currentLanguage = _uiSettingService.CurrentLanguage;

        if (UiElementTooltip != null)
        {
            var effectText = GetEffectText(currentLanguage, _diseaseSymptomEffect);
            UiElementTooltip.text = effectText;
        }
    }

    private static string GetEffectText(Language currentLanguage, DiseaseSymptomEffect diseaseSymptomEffect)
    {
        var symptomName = LocalizationHelper.GetValueOrDefaultNoname(currentLanguage, diseaseSymptomEffect.Symptom.Name);

        var diseaseNames = new List<string>();

        foreach (var disease in diseaseSymptomEffect.Diseases)
        {
            var diseaseName = DiseaseLocalizationHelper.GetValueOrDefaultNoname(currentLanguage, disease.Name);

            diseaseNames.Add(diseaseName);
        }

        return $"{symptomName}\n{string.Join("\n", diseaseNames)}";
    }
}
