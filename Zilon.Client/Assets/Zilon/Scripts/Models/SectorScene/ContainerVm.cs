using System;
using UnityEngine;
using Zilon.Core.Tactics;

public class ContainerVm : MonoBehaviour
{

	public event EventHandler Selected;

	public IPropContainer Container { get; set; }

	public void OnMouseDown()
	{
		DoSelected();
	}

	private void DoSelected()
	{
		Selected?.Invoke(this, new EventArgs());
	}
}
