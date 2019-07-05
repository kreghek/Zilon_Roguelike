using UnityEngine;
using UnityEngine.UI;

using Zilon.Core.PersonDialogs;

public sealed class DialogTransitionViewModel : MonoBehaviour
{
    public Text TransitionText;

    public DialogTransition DialogTransition { get; set; }

    public void Init(DialogTransition dialogTransition)
    {
        DialogTransition = dialogTransition;
        TransitionText.text = dialogTransition.Text;
    }
}

