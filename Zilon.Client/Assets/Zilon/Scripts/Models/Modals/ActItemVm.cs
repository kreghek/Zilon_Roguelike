using System;

using Assets.Zilon.Scripts.Models;

using UnityEngine;
using UnityEngine.UI;

using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

public class ActItemVm : MonoBehaviour, IActViewModelDescription
{
    public event EventHandler Click;
    public event EventHandler MouseEnter;
    public event EventHandler MouseExit;

    public ITacticalAct Act { get; protected set; }

    public Vector3 Position => GetComponent<RectTransform>().position;

    public GameObject SelectedBorder;

    public Image IconImage;

    public GameObject CooldownMarker;

    public void Init(ITacticalAct act)
    {
        Act = act;

        var icon = CalcIcon(act.Scheme, act.Equipment);
        IconImage.sprite = icon;
    }

    private Sprite CalcIcon(ITacticalActScheme scheme, Equipment equipment)
    {
        var schemeSid = scheme.Sid;
        if (scheme.IsMimicFor != null)
        {
            schemeSid = scheme.IsMimicFor;
        }

        var iconFromActs = Resources.Load<Sprite>($"Icons/acts/{schemeSid}");
        if (iconFromActs == null)
        {
            if (equipment != null)
            {
                var iconFromProps = Resources.Load<Sprite>($"Icons/props/{equipment.Scheme.Sid}");
                if (iconFromProps == null)
                {
                    var defaultIcon = Resources.Load<Sprite>($"Icons/acts/default");
                    return defaultIcon;
                }
                else
                {
                    return iconFromProps;
                }
            }
            else
            {
                var defaultIcon = Resources.Load<Sprite>($"Icons/acts/default");
                return defaultIcon;
            }
        }
        else
        {
            return iconFromActs;
        }
    }

    public void Click_Handler()
    {
        Click?.Invoke(this, new EventArgs());
    }

    public void SetSelectedState(bool selected)
    {
        SelectedBorder.SetActive(selected);
    }

    public void OnMouseEnter()
    {
        MouseEnter?.Invoke(this, new EventArgs());
    }

    public void OnMouseExit()
    {
        MouseExit?.Invoke(this, new EventArgs());
    }

    public void Update()
    {
        if (Act == null)
        {
            return;
        }

        if (Act.CurrentCooldown > 0)
        {
            CooldownMarker.SetActive(true);
        }
        else
        {
            CooldownMarker.SetActive(false);
        }
    }
}
