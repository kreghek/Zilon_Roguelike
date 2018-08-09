using Assets.Zilon.Scripts;
using System;

using UnityEngine;

public class ModalDialog : MonoBehaviour
{
    public GameObject Body;
    public IModalWindowHandler WindowHandler { get; set; }

    public event EventHandler AcceptChanges;
    
    public void Close()
    {
        WindowHandler.ApplyChanges();
        AcceptChanges?.Invoke(this, new EventArgs());
        Destroy(gameObject);
    }
}