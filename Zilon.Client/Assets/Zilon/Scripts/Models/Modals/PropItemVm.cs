using System;

using Assets.Zilon.Scripts.Models;
using Assets.Zilon.Scripts.Models.Modals;

using UnityEngine;
using UnityEngine.UI;

using Zilon.Core.Client;
using Zilon.Core.Props;

public sealed class PropItemVm : PropItemViewModelBase, IPropItemViewModel, IPropViewModelDescription
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

    public void Click_Handler()
    {
        Click?.Invoke(this, new EventArgs());
    }

    public void OnMouseEnter()
    {
        MouseEnter?.Invoke(this, new EventArgs());
    }

    public void OnMouseExit()
    {
        MouseExit?.Invoke(this, new EventArgs());
    }

    public void UpdateProp()
    {
        if (Prop is Resource resource)
        {
            CountText.gameObject.SetActive(true);
            CountText.text = $"x{resource.Count}";

            DurableStatusText.gameObject.SetActive(false);
        }
        else if (Prop is Equipment equipment)
        {
            CountText.gameObject.SetActive(false);

            if (equipment.Durable.Value <= 0)
            {
                DurableStatusText.gameObject.SetActive(true);
                DurableStatusText.text = "B";
            }
            else
            {
                DurableStatusText.gameObject.SetActive(false);
            }
        }
        else
        {
            throw new ArgumentException($"Тип предмета {Prop.GetType().Name} не поддерживается", nameof(Prop));
        }

        Sid = Prop.Scheme.Sid;

        var iconSprite = CalcIcon(Prop);

        IconImage.sprite = iconSprite;
    }

    private Sprite CalcIcon(IProp prop)
    {
        var schemeSid = prop.Scheme.Sid;
        if (prop.Scheme.IsMimicFor != null)
        {
            schemeSid = prop.Scheme.IsMimicFor;
        }

        var iconSprite = Resources.Load<Sprite>($"Icons/props/{schemeSid}");
        return iconSprite;
    }
}
