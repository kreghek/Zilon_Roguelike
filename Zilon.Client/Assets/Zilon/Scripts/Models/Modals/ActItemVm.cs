using System;

using UnityEngine;

using Zilon.Core.Persons;

public class ActItemVm : MonoBehaviour
{
	public event EventHandler Click;

	public ITacticalAct Act { get; set; }

	public GameObject SelectedBorder;
	
	public void Init(ITacticalAct act)
	{
		Act = act;
	}

	public void Click_Handler()
	{
		Click?.Invoke(this, new EventArgs());
	}

	public void SetSelectedState(bool selected)
	{
		SelectedBorder.SetActive(selected);
	}
}
