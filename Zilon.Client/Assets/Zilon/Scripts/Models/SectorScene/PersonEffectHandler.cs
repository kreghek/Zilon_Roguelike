using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;

public class PersonEffectHandler : MonoBehaviour
{
    [UsedImplicitly]
    [NotNull] [Inject] private readonly IPlayerState _playerState;

    [UsedImplicitly]
    [NotNull] [Inject] private readonly IActorManager _actorManager;

    public Transform EffectParent;
    public EffectViewModel EffectPrefab;

    [UsedImplicitly]
    public void Start()
    {
        UpdateEffects();

        var person = _playerState.ActiveActor.Actor.Person;
        person.Survival.StatCrossKeyValue += Survival_StatCrossKeyValue;
    }

    public void OnDestroy()
    {
        // Делаем так, потому что при смене сектора _playerState.ActiveActor может быть обнулён.
        foreach (var actor in _actorManager.Items)
        {
            actor.Person.Survival.StatCrossKeyValue -= Survival_StatCrossKeyValue;
        }
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

        var person = _playerState.ActiveActor.Actor.Person;

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
