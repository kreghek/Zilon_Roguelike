using System;

using UnityEngine;
using UnityEngine.UI;

using Zilon.Core.PersonDialogs;

public sealed class DialogTransitionViewModel : MonoBehaviour
{
    public Text TransitionText;

    public DialogTransition DialogTransition { get; set; }

    public event EventHandler Clicked;

    public void Init(DialogTransition dialogTransition)
    {
        DialogTransition = dialogTransition;
        TransitionText.text = dialogTransition.Text;
    }

    public void ButtonClick_Handler()
    {
        Clicked?.Invoke(this, new EventArgs());
    }
}

