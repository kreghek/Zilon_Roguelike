using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zilon.Core.Persons;

public class ActItemVm : MonoBehaviour
{

	public event EventHandler Click;
	public ITacticalAct Act { get; set; }
	
	public void Init(ITacticalAct act)
	{
		Act = act;
	}

	public void Click_Handler()
	{
		Click?.Invoke(this, new EventArgs());
	}
}
