using Assets.Zilon.Scripts.Services;
using UnityEngine;
using Zilon.Core.Client;

public class SectorModalManager : MonoBehaviour, ISectorModalManager
{
	public GameObject WindowsParent;
	
	public ModalDialog ModalPrefab;
	
	public ShowContainerModalBody ShowContainerModalPrefab;

	public void ShowContainerModal(PropTransferMachine transferMachine)
	{
		var modal = Instantiate(ModalPrefab, WindowsParent.transform);

		var containerModalBody = Instantiate(ShowContainerModalPrefab, modal.Body.transform);
		containerModalBody.SetTransferMachine(transferMachine);
	}
}
