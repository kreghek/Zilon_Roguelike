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
            if (_windowHandler != null)
            {
                _windowHandler.Closed -= WindowHandler_Closed;
            }

            _windowHandler = value;
            CaptionText.text = _windowHandler.Caption;

            _windowHandler.Closed += WindowHandler_Closed;
        }
    }

    private void WindowHandler_Closed(object sender, EventArgs e)
    {
        Close();
    }

    public event EventHandler AcceptChanges;

    public void Close()
    {
        WindowHandler.ApplyChanges();
        AcceptChanges?.Invoke(this, new EventArgs());
        Destroy(gameObject);
    }
}