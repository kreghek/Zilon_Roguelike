using System;

using Assets.Zilon.Scripts;

using UnityEngine;
using UnityEngine.UI;

public class ModalDialog : MonoBehaviour
{
    public Text CaptionText;
    public GameObject Body;
    private IModalWindowHandler _windowHandler;

    private void Start()
    {
        CaptionText.text = _windowHandler.Caption;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    private void OnDestroy()
    {
        if (_windowHandler != null)
        {
            _windowHandler.Closed -= WindowHandler_Closed;
        }
    }

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
        if (WindowHandler is null)
        {
            Debug.LogError("Try to close the modal before assign window handler.");
            Destroy(gameObject);
            return;
        }

        switch (WindowHandler.CloseBehaviour)
        {
            case CloseBehaviourOperation.ApplyChanges:
                WindowHandler.ApplyChanges();
                AcceptChanges?.Invoke(this, new EventArgs());
                break;

            case CloseBehaviourOperation.DoNothing:
                // Do nothing. Just close. By default all changes will lost.
                break;

            default:
                Debug.LogError($"Unexpected value {WindowHandler.CloseBehaviour}");
                break;
        }

        Destroy(gameObject);
    }
}