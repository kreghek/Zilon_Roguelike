using System;

using Assets.Zilon.Scripts;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;

public class PerksModalBody : MonoBehaviour, IModalWindowHandler
{

    private IActor _actor;

    public Transform PerkItemsParent;
    public PerkItemViewModel PerkItemPrefab;
    public PerkInfoPopup PerkInfoPopup;

    public event EventHandler Closed;

    public string Caption => "Character";

    public void OnDestroy()
    {
        ClearParentContent(PerkItemsParent);
    }

    public void Init(IActor actor)
    {
        _actor = actor;
        var evolutionData = _actor.Person.EvolutionData;
        UpdatePerksInner(PerkItemsParent, evolutionData.Perks);
    }

    private void UpdatePerksInner(Transform itemsParent, IPerk[] perks)
    {
        ClearParentContent(itemsParent);

        foreach (var perk in perks)
        {
            var perkItemVm = Instantiate(PerkItemPrefab, itemsParent);
            perkItemVm.Init(perk);
            perkItemVm.MouseEnter += PerkItemVm_MouseEnter;
            perkItemVm.MouseExit += PerkItemVm_MouseExit;
        }
    }

    private void ClearParentContent(Transform itemsParent)
    {
        foreach (Transform itemTranform in itemsParent)
        {
            var perkViewModel = itemTranform.GetComponent<PerkItemViewModel>();
            perkViewModel.MouseEnter -= PerkItemVm_MouseEnter;
            perkViewModel.MouseExit -= PerkItemVm_MouseExit;

            Destroy(itemTranform.gameObject);
        }
    }

    private void PerkItemVm_MouseExit(object sender, EventArgs e)
    {
        PerkInfoPopup.SetPropViewModel(null);
    }

    private void PerkItemVm_MouseEnter(object sender, EventArgs e)
    {
        var currentItemVm = (PerkItemViewModel)sender;
        PerkInfoPopup.SetPropViewModel(currentItemVm);
    }

    public void ApplyChanges()
    {

    }

    public void CancelChanges()
    {
        throw new System.NotImplementedException();
    }
}
