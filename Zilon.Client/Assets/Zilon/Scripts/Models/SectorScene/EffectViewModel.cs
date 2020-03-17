using System;

using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Persons;

public class EffectViewModel : MonoBehaviour
{
    public Image EffectIcon;
    public Image Background;
    public Text NameText;

    public Sprite HungerSprite;
    public Sprite ThristSprite;
    public Sprite IntoxicationSprite;
    public Sprite InjureSprite;

    public UiElementTooltip UiElementTooltip;

    [Inject]
    UiSettingService _uiSettingService;

    public SurvivalStatType Type { get; private set; }
    public SurvivalStatHazardLevel Level { get; private set; }

    public void Init(SurvivalStatType type, SurvivalStatHazardLevel level)
    {
        var currentLanguage = _uiSettingService.CurrentLanguage;

        Type = type;
        Level = level;
        SelectIcon(type);
        HighlightLevel(level);
        ShowText(currentLanguage);

        if (UiElementTooltip != null)
        {
            var effectText = GetEffectText(currentLanguage);
            UiElementTooltip.text = effectText;
        }
    }

    private void ShowText(Language currentLanguage)
    {
        var effectText = GetEffectText(currentLanguage);

        NameText.text = effectText;
    }

    private string GetEffectText(Language currentLanguage)
    {
        var effectText = string.Empty;
        switch (Level)
        {
            case SurvivalStatHazardLevel.Lesser:
                effectText = GetLesserEffect(currentLanguage);

                NameText.color = Color.gray;
                break;

            case SurvivalStatHazardLevel.Strong:
                effectText = GetStrongEffect(currentLanguage);

                NameText.color = Color.red;
                break;

            case SurvivalStatHazardLevel.Max:
                effectText = GetMaxEffect(currentLanguage);
                NameText.color = Color.red;
                break;
        }

        return effectText;
    }

    private string GetMaxEffect(Language currentLanguage)
    {
        switch (Type)
        {
            case SurvivalStatType.Health:
                return StaticPhrases.GetValue("max-injury", currentLanguage);

            case SurvivalStatType.Satiety:
                return StaticPhrases.GetValue("max-hunger", currentLanguage);

            case SurvivalStatType.Hydration:
                return StaticPhrases.GetValue("max-thirst", currentLanguage);

            case SurvivalStatType.Intoxication:
                return StaticPhrases.GetValue("max-intoxication", currentLanguage);

            default:
                throw new InvalidOperationException();
        }
    }

    private string GetStrongEffect(Language currentLanguage)
    {
        switch (Type)
        {
            case SurvivalStatType.Health:
                return StaticPhrases.GetValue("strong-injury", currentLanguage);

            case SurvivalStatType.Satiety:
                return StaticPhrases.GetValue("strong-hunger", currentLanguage);

            case SurvivalStatType.Hydration:
                return StaticPhrases.GetValue("strong-thirst", currentLanguage);

            case SurvivalStatType.Intoxication:
                return StaticPhrases.GetValue("strong-intoxication", currentLanguage);

            default:
                throw new InvalidOperationException();
        }
    }

    private string GetLesserEffect(Language currentLanguage)
    {
        switch (Type)
        {
            case SurvivalStatType.Health:
                return StaticPhrases.GetValue("weak-injury", currentLanguage);

            case SurvivalStatType.Satiety:
                return StaticPhrases.GetValue("weak-hunger", currentLanguage);

            case SurvivalStatType.Hydration:
                return StaticPhrases.GetValue("weak-thirst", currentLanguage);

            case SurvivalStatType.Intoxication:
                return StaticPhrases.GetValue("weak-intoxication", currentLanguage);

            default:
                throw new InvalidOperationException();
        }
    }

    private void HighlightLevel(SurvivalStatHazardLevel level)
    {
        switch (level)
        {
            case SurvivalStatHazardLevel.Lesser:
                Background.color = new Color32(0xff, 0xcc, 0x5e, 255);
                break;

            case SurvivalStatHazardLevel.Strong:
                Background.color = new Color32(0xff, 0x7c, 0x24, 255);
                break;

            case SurvivalStatHazardLevel.Max:
                Background.color = new Color32(0xf3, 0x28, 0x13, 255);
                break;

            default:
                throw new InvalidOperationException();
        }
    }

    private void SelectIcon(SurvivalStatType type)
    {
        switch (type)
        {
            case SurvivalStatType.Health:
                EffectIcon.sprite = InjureSprite;
                break;

            case SurvivalStatType.Satiety:
                EffectIcon.sprite = HungerSprite;
                break;

            case SurvivalStatType.Hydration:
                EffectIcon.sprite = ThristSprite;
                break;

            case SurvivalStatType.Intoxication:
                EffectIcon.sprite = IntoxicationSprite;
                break;

            default:
                throw new InvalidOperationException();
        }
    }
}
