using System;
using System.Collections.Generic;
using Assets.Zilon.Scripts.Models.SectorScene;
using UnityEngine;
using Zenject;
using Zilon.Core.Persons;

public class ActPanelHandler : MonoBehaviour {

	[Inject] private IPlayerState _playerState;

	public ActItemVm ActVmPrefab;
	public Transform ActItemParent;

	public void Start()
	{
		var actorVm = _playerState.ActiveActor;
		var actor = actorVm.Actor;
		
		actor.Person.EquipmentCarrier.EquipmentChanged += EquipmentCarrierOnEquipmentChanged;
		
		var acts = actor.Person.TacticalActCarrier.Acts;
		UpdateActs(acts);
	}

	private void EquipmentCarrierOnEquipmentChanged(object sender, EquipmentChangedEventArgs e)
	{
		var actorVm = _playerState.ActiveActor;
		var actor = actorVm.Actor;
		
		var acts = actor.Person.TacticalActCarrier.Acts;
		UpdateActs(acts);
	}

	private void UpdateActs(IEnumerable<ITacticalAct> acts)
	{
		foreach (Transform item in ActItemParent)
		{
			Destroy(item.gameObject);
		}

		foreach (var act in acts)
		{
			var actItemVm = Instantiate(ActVmPrefab, ActItemParent);
			actItemVm.Init(act);
			actItemVm.Click += ActClick_Handler;
		}
	}

	private void ActClick_Handler(object sender, EventArgs e)
	{
		var actItemVm = sender as ActItemVm;
		Debug.Log(actItemVm.Act.Scheme.Sid);
	}
}
