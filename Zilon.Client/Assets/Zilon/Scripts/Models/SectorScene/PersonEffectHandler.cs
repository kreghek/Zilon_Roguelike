using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Persons;
using Zilon.Core.Players;

public class PersonEffectHandler : MonoBehaviour
{
    [UsedImplicitly]
    [NotNull] [Inject] private readonly HumanPlayer _player;

    [Inject]
    private DiContainer _diContainer;

    public Transform EffectParent;
    public EffectViewModel EffectPrefab;

    [UsedImplicitly]
    public void Start()
    {
        UpdateEffects();

        var person = _player.MainPerson;

//TODO Не очень надёжное решение.
// Будет проблема, если этот скрипт будет запущен перед скриптом создания персонажа.
        if (person != null)
        {
            person.Survival.StatChanged += Survival_StatChanged;
        }
    }

    public void OnDestroy()
    {
        var person = _player.MainPerson;

        if (person != null)
        {
            person.Survival.StatChanged -= Survival_StatChanged;
        }
    }

    private void Survival_StatChanged(object sender, SurvivalStatChangedEventArgs e)
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

        if (person == null)
        {
            return;
        }

        var effects = person.Effects;

        foreach (var effect in effects.Items)
        {
            if (effect is SurvivalStatHazardEffect survivalHazardEffect)
            {
                var effectViewModelObj = _diContainer.InstantiatePrefab(EffectPrefab, EffectParent);
                var effectViewModel = effectViewModelObj.GetComponent<EffectViewModel>();
                effectViewModel.Init(survivalHazardEffect.Type, survivalHazardEffect.Level);
            }
        }
    }
}
