using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Persons;

public class PersonEffectHandler : MonoBehaviour
{
    private IPerson _person;

    [UsedImplicitly]
    [NotNull]
    [Inject]
    private readonly ISectorUiState _sectorUiState;

    [Inject]
    private DiContainer _diContainer;

    public Transform EffectParent;
    public EffectViewModel EffectPrefab;
    private TaskScheduler _taskScheduler;

    [UsedImplicitly]
    private void Start()
    {
        _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        _sectorUiState.ActiveActorChanged += SectorState_ActiveActorChanged;
    }

    private void OnDestroy()
    {
        _sectorUiState.ActiveActorChanged -= SectorState_ActiveActorChanged;

        DropCurrentPersonSubscribtions();
    }

    private void SectorState_ActiveActorChanged(object sender, System.EventArgs e)
    {
        var newPerson = _sectorUiState.ActiveActor.Actor.Person;
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
        // Этот код обработчика должен выполниться в потоке Unity и не важно в каком потоке было выстелено событие.
        // https://stackoverflow.com/questions/40733647/how-to-call-event-handler-through-ui-thread-when-the-operation-is-executing-into
        Task.Factory.StartNew(() =>
        {
            UpdateEffects(_person);
        }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
    }

    private void UpdateEffects(IPerson person)
    {
        ClearCurrentEffectViewModels();
        CreateEffectsOfCurrentPerson(person);
    }

    private void CreateEffectsOfCurrentPerson(IPerson person)
    {
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
                var effectViewModelObj = _diContainer.InstantiatePrefab(EffectPrefab, EffectParent);
                var effectViewModel = effectViewModelObj.GetComponent<EffectViewModel>();
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
