using System;

using UnityEngine;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;

public class MonsterSingleActorGraphicController : MonoBehaviour
{
    public IActor Actor { get; set; }
    public ActorGraphicBase Graphic;

    public void Start()
    {
        if (Actor.Person == null)
        {
            throw new InvalidOperationException("Не указан персонаж.");
        }

        var monsterPerson = Actor.Person as MonsterPerson;
        if (monsterPerson == null)
        {
            throw new NotSupportedException($"Тип персонажа {Actor.Person} не поддерживается ");
        }

        Debug.Log(monsterPerson.Scheme.Sid);
        SetVisualProp(monsterPerson.Scheme.Sid, 0);
    }

    private void SetVisualProp(string propSid, int slotIndex)
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
}
