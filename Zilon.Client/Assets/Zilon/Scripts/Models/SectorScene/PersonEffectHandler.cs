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

    [UsedImplicitly]
    public void Start()
    {
        UpdateEffects();

        var person = _player.MainPerson;
        person.Survival.StatCrossKeyValue += Survival_StatCrossKeyValue;
    }

    public void OnDestroy()
    {
        _player.MainPerson.Survival.StatCrossKeyValue -= Survival_StatCrossKeyValue;
    }

    private void Survival_StatCrossKeyValue(object sender, SurvivalStatChangedEventArgs e)
    {
        UpdateEffects();
    }

    private void UpdateEffects()
    {
        foreach (Transform childTrasform in EffectParent)
        {
            Destroy(childTrasform.gameObject);
        }

        var person = _player.MainPerson;

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
