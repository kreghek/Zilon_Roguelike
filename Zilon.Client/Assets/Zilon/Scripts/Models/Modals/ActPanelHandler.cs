using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Persons;

public class ActPanelHandler : MonoBehaviour
{
    private readonly List<ActItemVm> _actViewModels;

    [Inject] private readonly ISectorUiState _playerState;

    public ActItemVm ActVmPrefab;

    public Transform ActItemParent;

    public ActInfoPopup ActInfoPopup;

    public ActPanelHandler()
    {
        _actViewModels = new List<ActItemVm>();
    }

    public void Start()
    {
        var actorVm = _playerState.ActiveActor;
        var actor = actorVm.Actor;

        actor.Person.EquipmentCarrier.EquipmentChanged += EquipmentCarrierOnEquipmentChanged;

        var acts = actor.Person.TacticalActCarrier.Acts;

        UpdateSelectedAct(currentAct: null, acts);

        UpdateActs(acts);
    }

    private void EquipmentCarrierOnEquipmentChanged(object sender, EquipmentChangedEventArgs e)
    {
        var currentAct = _playerState.TacticalAct;
        _playerState.TacticalAct = null;

        var actorVm = _playerState.ActiveActor;
        var actor = actorVm.Actor;

        var acts = actor.Person.TacticalActCarrier.Acts;
        UpdateActs(acts);
        UpdateSelectedAct(currentAct, acts);
    }

    private void UpdateSelectedAct(ITacticalAct currentAct, ITacticalAct[] acts)
    {
        if (acts.Contains(currentAct))
        {
            _playerState.TacticalAct = currentAct;
        }
        else
        {
            _playerState.TacticalAct = acts.First();
        }
    }

    private void UpdateActs(IEnumerable<ITacticalAct> acts)
    {
        foreach (Transform item in ActItemParent)
        {
            Destroy(item.gameObject);
        }

        _actViewModels.Clear();
        var actArray = acts.ToArray();
        foreach (var act in actArray)
        {
            var actItemVm = Instantiate(ActVmPrefab, ActItemParent);
            actItemVm.Init(act);
            actItemVm.Click += ActClick_Handler;
            _actViewModels.Add(actItemVm);
        }
    }

    private void ActClick_Handler(object sender, EventArgs e)
    {
        var actItemVm = sender as ActItemVm;
        if (actItemVm == null)
        {
            throw new InvalidOperationException("Не указано действие (ViewModel).");
        }

        var selectedAct = GetAct(actItemVm);

        _playerState.TacticalAct = selectedAct;

        foreach (var actVm in _actViewModels)
        {
            var isSelected = actVm.Act == selectedAct;
            actVm.SetSelectedState(isSelected);
        }

        Debug.Log(selectedAct);
    }

    private static TacticalAct GetAct(ActItemVm actItemVm)
    {
        if (actItemVm.Act is TacticalAct act)
        {
            return act;
        }

        throw new InvalidOperationException("Не указано действие (Domain).");
    }
}
