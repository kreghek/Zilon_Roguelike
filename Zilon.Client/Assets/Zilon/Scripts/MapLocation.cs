using System;
using UnityEngine;

public class MapLocation : MonoBehaviour
{

    public string Sid { get; set; }

    public ModalDialog DialogPrefab;
    public Canvas Canvas;
    public event EventHandler OnSelect;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseDown()
    {
        OnSelect(this, new EventArgs());
    }

    internal void SetAvailableState(bool v)
    {
        throw new NotImplementedException();
    }
}
