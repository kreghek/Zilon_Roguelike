using System.Linq;
using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Persons;
using Zilon.Core.Players;

public class PersonEffectHandler : MonoBehaviour
{
    [UsedImplicitly]
    [NotNull] [Inject] private readonly HumanPlayer _player;

    public Transform EffectParent;
    public EffectViewModel EffectPrefab;

    public int Intoxication;

    [UsedImplicitly]
    public void Start()
    {
        UpdateEffects();

        var person = _player.MainPerson;

//TODO Не очень надёжное решение.
// Будет проблема, если этот скрипт будет запущен перед скриптом создания персонажа.
        if (person != null)
        {
            person.Survival.StatCrossKeyValue += Survival_StatCrossKeyValue;
        }
    }

    public void Update()
    {
        var person = _player.MainPerson;
        if (person != null)
        {
            Intoxication = person.Survival.Stats.Single(x => x.Type == SurvivalStatType.Intoxication).Value;
        }
    }

    public void OnDestroy()
    {
        var person = _player.MainPerson;

        if (person != null)
        {
            person.Survival.StatCrossKeyValue -= Survival_StatCrossKeyValue;
        }
    }

    private void Survival_StatCrossKeyValue(object sender, SurvivalStatChangedEventArgs e)
    {
        if (e.Stat.Type == SurvivalStatType.Intoxication)
        {
            foreach (var keyPoint in e.KeyPoints)
            {
                Debug.Log(keyPoint.Level + " " + keyPoint.Value);
            }
        }

        UpdateEffects();
    }

    private void UpdateEffects()
    {
        foreach (Transform childTrasform in EffectParent)
        {
            Destroy(childTrasform.gameObject);
        }

        var person = _player.MainPerson;

        if (person == null)
        {
            return;
        }

        var effects = person.Effects;

        foreach (var effect in effects.Items)
        {
            if (effect is SurvivalStatHazardEffect survivalHazardEffect)
            {
                var effectViewModel = Instantiate(EffectPrefab, EffectParent);
                effectViewModel.Init(survivalHazardEffect.Type, survivalHazardEffect.Level);
            }
        }
    }
}
