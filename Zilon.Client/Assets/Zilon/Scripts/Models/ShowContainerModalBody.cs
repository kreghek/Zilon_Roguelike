using UnityEngine;
using Zilon.Core.Persons;

public class ShowContainerModalBody : MonoBehaviour
{
	public GameObject PropItemPrefab;
	public Transform ItemsParent;

	public void SetProps(IProp[] props)
	{
		foreach (var prop in props)
		{
			Instantiate(PropItemPrefab, ItemsParent);
		}
	}
}
