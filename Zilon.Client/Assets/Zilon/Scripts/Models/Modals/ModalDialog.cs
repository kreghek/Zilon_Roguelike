using System;

using Assets.Zilon.Scripts;

using UnityEngine;
using UnityEngine.UI;

public class ModalDialog : MonoBehaviour
{
    public Text CaptionText;
    public GameObject Body;
    private IModalWindowHandler _windowHandler;

    public IModalWindowHandler WindowHandler
    {
        get => _windowHandler;
        set
        {
            _windowHandler = value;
            CaptionText.text = _windowHandler.Caption;
        }
    }

    public event EventHandler AcceptChanges;

    public void Close()
    {
        WindowHandler.ApplyChanges();
        AcceptChanges?.Invoke(this, new EventArgs());
        Destroy(gameObject);
    }
}