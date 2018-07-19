using Assets.Zilon.Scripts.Models.SectorScene;
using Assets.Zilon.Scripts.Services;
using UnityEngine;
using Zenject;
using Zilon.Core.Client;
using Zilon.Core.Tactics;

public class SectorModalManager : MonoBehaviour, ISectorModalManager
{
	public GameObject WindowsParent;
	
	public ModalDialog ModalPrefab;
	
	public ShowContainerModalBody ShowContainerModalPrefab;
	
	public InventoryModalBody InventoryModalPrefab;

	public void ShowContainerModal(PropTransferMachine transferMachine)
	{
		var modal = Instantiate(ModalPrefab, WindowsParent.transform);

		var modalBody = Instantiate(ShowContainerModalPrefab, modal.Body.transform);

        modal.WindowHandler = modalBody;

		modalBody.SetTransferMachine(transferMachine);
	}

	public void ShowInventoryModal(IActor actor)
	{
		var modal = Instantiate(ModalPrefab, WindowsParent.transform);

		var modalBody = Instantiate(InventoryModalPrefab, modal.Body.transform);

		modal.WindowHandler = modalBody;
		
		modalBody.Init(actor);
	}
}
