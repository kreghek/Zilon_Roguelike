using UnityEngine;
using Zilon.Core.Persons;

public class DiseaseEffectViewModel : MonoBehaviour
{
    public UiElementTooltip UiElementTooltip;

    public void Init(IDisease disease)
    {
        var currentLanguage = _uiSettingService.CurrentLanguage;

        

        if (UiElementTooltip != null)
        {
            var effectText = GetEffectText(currentLanguage);
            UiElementTooltip.text = effectText;
        }
    }
}
