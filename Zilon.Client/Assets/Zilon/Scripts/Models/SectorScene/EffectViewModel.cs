using System;

using UnityEngine;
using UnityEngine.UI;

using Zilon.Core.Persons;

public class EffectViewModel : MonoBehaviour
{
    public Image EffectIcon;
    public Image Background;

    public Sprite HungerSprite;
    public Sprite ThristSprite;

    public SurvivalStatType Type { get; private set; }
    public SurvivalStatHazardLevel Level { get; private set; }

    public void Init(SurvivalStatType type, SurvivalStatHazardLevel level)
    {
        Type = type;
        Level = level;
        SelectIcon(type);
        HighlightLevel(level);
    }

    private void HighlightLevel(SurvivalStatHazardLevel level)
    {
        switch (level)
        {
            case SurvivalStatHazardLevel.Lesser:
                Background.color = Color.white;
                break;

            case SurvivalStatHazardLevel.Strong:
                Background.color = new Color(0.3f, 0,0, 1);
                break;

            case SurvivalStatHazardLevel.Max:
                Background.color = new Color(1f, 0, 0, 1);
                break;

            default:
                throw new InvalidOperationException();
        }
    }

    private void SelectIcon(SurvivalStatType type)
    {
        switch (type)
        {
            case SurvivalStatType.Satiety:
                EffectIcon.sprite = HungerSprite;
                break;

            case SurvivalStatType.Water:
                EffectIcon.sprite = ThristSprite;
                break;

            default:
                throw new InvalidOperationException();
        }
    }
}
