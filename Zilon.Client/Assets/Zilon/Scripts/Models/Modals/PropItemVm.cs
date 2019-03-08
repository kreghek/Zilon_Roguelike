using System;

using Assets.Zilon.Scripts.Models;

using UnityEngine;
using UnityEngine.UI;

using Zilon.Core.Client;
using Zilon.Core.Props;

public class PropItemVm : MonoBehaviour, IPropItemViewModel, IPropViewModelDescription
{
    public Text CountText;
    public Text DurableStatusText;
    public Image IconImage;
    public Image SelectedBorder;

    public string Sid;

    public IProp Prop { get; private set; }
    public Vector3 Position => GetComponent<RectTransform>().position;

    public event EventHandler Click;
    public event EventHandler MouseEnter;
    public event EventHandler MouseExit;

    public void Init(IProp prop)
    {
        Prop = prop;

        UpdateProp();
    }

    public void SetSelectedState(bool value)
    {
        SelectedBorder.gameObject.SetActive(value);
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
        var iconSprite = Resources.Load<Sprite>($"Icons/props/{prop.Scheme.Sid}");
        return iconSprite;
    }
}
