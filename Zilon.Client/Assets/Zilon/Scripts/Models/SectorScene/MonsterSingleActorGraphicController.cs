using System;

using UnityEngine;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;

public class MonsterSingleActorGraphicController : MonoBehaviour
{
    public IActor Actor { get; set; }

    private const int TRADER_VISUAL_COUNT = 4;
    public ActorGraphicBase Graphic;

    public void Start()
    {
        if (Actor.Person == null)
        {
            throw new InvalidOperationException("Не указан персонаж.");
        }

        switch (Actor.Person)
        {
            case MonsterPerson monsterPerson:
                SetMonsterVisualProp(monsterPerson.Scheme.Sid);
                break;

            case CitizenPerson _:
                // +1, потому что рандом по целым числам исключет максимум.
                const int maxIndexRandomValue = TRADER_VISUAL_COUNT + 1;
                var visualIndex = UnityEngine.Random.Range(1, maxIndexRandomValue);
                SetCitizenVisualProp(visualIndex);
                break;

            default:
                throw new NotSupportedException($"Тип персонажа {Actor.Person} не поддерживается ");
        }
    }

    private void SetMonsterVisualProp(string propSid)
    {
        var monoGraphics = (MonoActorGraphic)Graphic;
        var holder = monoGraphics.VisualPropHolder;
        var visualPropResource = Resources.Load<VisualProp>($"VisualProps/Monsters/{propSid}");
        if (visualPropResource != null)
        {
            Instantiate(visualPropResource, holder.transform);
        }
        else
        {
            visualPropResource = Resources.Load<VisualProp>($"VisualProps/Monsters/undef");
            Instantiate(visualPropResource, holder.transform);
        }
        
    }

    private void SetCitizenVisualProp(int traderVisualIndex)
    {
        var monoGraphics = (MonoActorGraphic)Graphic;
        var holder = monoGraphics.VisualPropHolder;
        var visualPropResource = Resources.Load<VisualProp>($"VisualProps/Traders/trader{traderVisualIndex}");
        if (visualPropResource != null)
        {
            Instantiate(visualPropResource, holder.transform);
        }
        else
        {
            visualPropResource = Resources.Load<VisualProp>($"VisualProps/Monsters/undef");
            Instantiate(visualPropResource, holder.transform);
        }

    }
}
