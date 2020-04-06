using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Players;

public class PersonEffectHandler : MonoBehaviour
{
    [UsedImplicitly]
    [NotNull]
    [Inject]
    private readonly HumanPlayer _player;

    [Inject]
    private readonly DiContainer _diContainer;

    public Transform EffectParent;

    public SurvivalHazardEffectViewModel SurvivalHazardEffectPrefab;

    public DiseaseEffectViewModel DiseaseEffectPrefab;

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
            switch (effect)
            {
                case SurvivalStatHazardEffect survivalHazardEffect:
                    CreateSurvivalHazardEffect(survivalHazardEffect);
                    break;

                case DiseaseSymptomEffect diseaseEffect:
                    CreateDiseaseEffect(diseaseEffect);
                    break;
            }
        }
    }

    private void CreateDiseaseEffect(DiseaseSymptomEffect diseaseEffect)
    {
        var effectViewModelObj = _diContainer.InstantiatePrefab(DiseaseEffectPrefab, EffectParent);
        var effectViewModel = effectViewModelObj.GetComponent<DiseaseEffectViewModel>();
        effectViewModel.Init(diseaseEffect);
    }

    private void CreateSurvivalHazardEffect(SurvivalStatHazardEffect survivalHazardEffect)
    {
        var effectViewModelObj = _diContainer.InstantiatePrefab(SurvivalHazardEffectPrefab, EffectParent);
        var effectViewModel = effectViewModelObj.GetComponent<SurvivalHazardEffectViewModel>();
        effectViewModel.Init(survivalHazardEffect.Type, survivalHazardEffect.Level);
    }
}
