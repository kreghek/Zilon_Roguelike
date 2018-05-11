using System;
using UnityEngine;
using Zilon.Core.Tactics.Map;

public class CombatLocationVM : MonoBehaviour {

    public MapNode Node { get; set; }

    public event EventHandler OnSelect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMouseDown()
    {
        OnSelect(this, new EventArgs());
    }
}
