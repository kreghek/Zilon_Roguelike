using System;

using UnityEngine;
using UnityEngine.UI;

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

    public SurvivalStatType Type { get; private set; }
    public SurvivalStatHazardLevel Level { get; private set; }

    public void Init(SurvivalStatType type, SurvivalStatHazardLevel level)
    {
        Type = type;
        Level = level;
        SelectIcon(type);
        HighlightLevel(level);
        ShowText();

        if (UiElementTooltip != null)
        {
            string effectText = GetEffectText();
            UiElementTooltip.text = effectText;
        }
    }

    private void ShowText()
    {
        string effectText = GetEffectText();

        NameText.text = effectText;
    }

    private string GetEffectText()
    {
        var effectText = string.Empty;
        switch (Level)
        {
            case SurvivalStatHazardLevel.Lesser:
                effectText = GetLesserEffect();

                NameText.color = Color.gray;
                break;

            case SurvivalStatHazardLevel.Strong:
                effectText = GetStrongEffect(effectText);

                NameText.color = Color.red;
                break;

            case SurvivalStatHazardLevel.Max:
                effectText = GetMaxEffect(effectText);
                NameText.color = Color.red;
                break;
        }

        return effectText;
    }

    private string GetMaxEffect(string effectText)
    {
        switch (Type)
        {
            case SurvivalStatType.Health:
                effectText = "Vital Wound!";
                break;

            case SurvivalStatType.Satiety:
                effectText = "Starvation!";
                break;

            case SurvivalStatType.Hydration:
                effectText = "Dehydration!";
                break;

            case SurvivalStatType.Intoxication:
                effectText += "Strong Intoxication!";
                break;
        }

        return effectText;
    }

    private string GetStrongEffect(string effectText)
    {
        switch (Type)
        {
            case SurvivalStatType.Health:
                effectText += "Strong Injury";
                break;

            case SurvivalStatType.Satiety:
                effectText = "Hunger";
                break;

            case SurvivalStatType.Hydration:
                effectText = "Thrist";
                break;

            case SurvivalStatType.Intoxication:
                effectText += "Intoxication";
                break;
        }

        return effectText;
    }

    private string GetLesserEffect()
    {
        string effectText = "Weak";
        switch (Type)
        {
            case SurvivalStatType.Health:
                effectText += " Injury";
                break;

            case SurvivalStatType.Satiety:
                effectText += " Hunger";
                break;

            case SurvivalStatType.Hydration:
                effectText += " Thrist";
                break;

            case SurvivalStatType.Intoxication:
                effectText += " Intoxication";
                break;
        }

        return effectText;
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
                Background.color = new Color32(0xc3, 0x28, 0x13, 255);
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
