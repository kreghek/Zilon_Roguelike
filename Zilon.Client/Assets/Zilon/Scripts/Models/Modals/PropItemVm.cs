using System;

using Assets.Zilon.Scripts.Models.Modals;

using UnityEngine;
using UnityEngine.UI;

using Zilon.Core.Props;

public sealed class PropItemVm : PropItemViewModelBase
{
    public Image SelectedBorder;

    public event EventHandler<PropDraggingStateEventArgs> DraggingStateChanged;

    public bool SelectAsDrag;

    public void Init(IProp prop)
    {
        Prop = prop;

        UpdateProp();
    }

    public void SetSelectedState(bool value)
    {
        SelectedBorder.gameObject.SetActive(value);
    }

    public void SetDraggingState(bool value)
    {
        SelectAsDrag = value;

        if (value)
        {
            IconImage.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            IconImage.color = new Color(1, 1, 1, 1f);
        }

        DraggingStateChanged?.Invoke(this, new PropDraggingStateEventArgs(value));
    }
}
