using Assets.Zilon.Scripts.Services;
using UnityEngine;
using Zilon.Core.Persons;

public class SectorModalManager : MonoBehaviour, ISectorModalManager
{
	public GameObject WindowsParent;
	
	public ModalDialog ModalPrefab;
	
	public GameObject ShowContainerModalPrefab;

	public void ShowContainerModal(IProp[] props)
	{
		var containerModal = Instantiate(ModalPrefab, WindowsParent.transform);
	}
}
