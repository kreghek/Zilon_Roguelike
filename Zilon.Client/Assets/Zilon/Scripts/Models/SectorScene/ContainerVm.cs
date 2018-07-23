using System;
using UnityEngine;
using Zilon.Core.Client;
using Zilon.Core.Tactics;

public class ContainerVm : MonoBehaviour, IContainerViewModel
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
