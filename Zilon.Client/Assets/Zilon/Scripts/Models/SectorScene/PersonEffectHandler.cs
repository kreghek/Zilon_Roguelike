using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Persons;

public class PersonEffectHandler : MonoBehaviour
{
    private IPerson _person;

    [UsedImplicitly]
    [NotNull] [Inject] 
    private readonly ISectorUiState _sectorState;

    public Transform EffectParent;
    public EffectViewModel EffectPrefab;

    [UsedImplicitly]
    private void Start()
    {
        _sectorState.ActiveActorChanged += SectorState_ActiveActorChanged;
    }

    private void OnDestroy()
    {
        _sectorState.ActiveActorChanged -= SectorState_ActiveActorChanged;

        DropCurrentPersonSubscribtions();
    }

    private void SectorState_ActiveActorChanged(object sender, System.EventArgs e)
    {
        var newPerson = _sectorState.ActiveActor.Actor.Person;
        HandlePersonChanged(newPerson);

        UpdateEffects(_person);

        //TODO Не очень надёжное решение.
        // Будет проблема, если этот скрипт будет запущен перед скриптом создания персонажа.
        if (_person != null)
        {
            _person.Survival.StatChanged += Survival_StatChanged;
        }
    }

    private void HandlePersonChanged(IPerson newPerson)
    {
        if (newPerson == _person)
        {
            return;
        }

        DropCurrentPersonSubscribtions();
        _person = newPerson;
    }

    private void DropCurrentPersonSubscribtions()
    {
        if (_person != null)
        {
            _person.Survival.StatChanged -= Survival_StatChanged;
        }
    }

    private void Survival_StatChanged(object sender, SurvivalStatChangedEventArgs e)
    {
        UpdateEffects(_person);
    }

    private void UpdateEffects(IPerson person)
    {
        ClearCurrentEffectViewModels();

        if (person == null)
        {
            return;
        }

        var effects = person.Effects;
        CreateEffectViewModels(effects);
    }

    private void CreateEffectViewModels(EffectCollection effects)
    {
        foreach (var effect in effects.Items)
        {
            if (effect is SurvivalStatHazardEffect survivalHazardEffect)
            {
                var effectViewModel = Instantiate(EffectPrefab, EffectParent);
                effectViewModel.Init(survivalHazardEffect.Type, survivalHazardEffect.Level);
            }
        }
    }

    private void ClearCurrentEffectViewModels()
    {
        foreach (Transform childTrasform in EffectParent)
        {
            Destroy(childTrasform.gameObject);
        }
    }
}
