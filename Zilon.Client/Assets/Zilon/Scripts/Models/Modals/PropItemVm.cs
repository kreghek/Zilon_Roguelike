using System;

using UnityEngine;
using UnityEngine.UI;

using Zilon.Core.Client;
using Zilon.Core.Props;

public class PropItemVm : MonoBehaviour, IPropItemViewModel
{
    public Text CountText;
    public Image IconImage;
    public Image SelectedBorder;

    public string Sid;

    public IProp Prop { get; private set; }

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

    //public void OnMouseEnter()
    //{
    //    MouseEnter?.Invoke(this, new EventArgs());
    //}

    //public void OnMouseExit()
    //{
    //    MouseExit?.Invoke(this, new EventArgs());
    //}

    public void UpdateProp()
    {
        if (Prop is Resource resource)
        {
            CountText.gameObject.SetActive(true);
            CountText.text = $"x{resource.Count}";
        }
        else
        {
            CountText.gameObject.SetActive(false);
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
