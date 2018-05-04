using System;
using UnityEngine;

public class MapLocation : MonoBehaviour
{
    public ModalDialog DialogPrefab;
    public Canvas Canvas;
    public event EventHandler OnSelect;
    public SpriteRenderer Icon;

    public string Sid { get; set; }

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

    internal void SetAvailableState(bool state)
    {
        Icon.color = state ? Color.white : Color.gray;
    }
}
