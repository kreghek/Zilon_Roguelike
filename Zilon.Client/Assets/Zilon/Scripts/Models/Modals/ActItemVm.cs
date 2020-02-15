using System;

using Assets.Zilon.Scripts.Models;

using UnityEngine;

using Zilon.Core.Persons;

public class ActItemVm : MonoBehaviour, IActViewModelDescription
{
	public event EventHandler Click;
	public event EventHandler MouseEnter;
	public event EventHandler MouseExit;

	public ITacticalAct Act { get; protected set; }

	public Vector3 Position => GetComponent<RectTransform>().position;

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

	public void OnMouseEnter()
	{
		MouseEnter?.Invoke(this, new EventArgs());
	}

	public void OnMouseExit()
	{
		MouseExit?.Invoke(this, new EventArgs());
	}
}
