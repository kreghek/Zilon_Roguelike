using System;

using UnityEngine;

using Zilon.Core.Client;
using Zilon.Core.World;

public class MapLocation : MonoBehaviour, IGlobeNodeViewModel
{
    public event EventHandler OnSelect;
    public event EventHandler OnHover;
    public SpriteRenderer Icon;

    public GlobeRegionNode Node { get; set; }

    public void Start()
    {
        if (Node.Scheme.Sid == "forest")
        {
            var sprite = Resources.Load<Sprite>("Globe/forest");
            Icon.sprite = sprite;
        }
        else if(Node.Scheme.Sid == "city")
        {
            var sprite = Resources.Load<Sprite>("Globe/city");
            Icon.sprite = sprite;
        }
        else
        {
            var sprite = Resources.Load<Sprite>("Globe/gungeon");
            Icon.sprite = sprite;
        }
    }

    public void OnMouseDown()
    {
        OnSelect?.Invoke(this, new EventArgs());
    }

    public void OnMouseOver()
    {
        OnHover?.Invoke(this, new EventArgs());
    }

    internal void SetAvailableState(bool state)
    {
        Icon.color = state ? Color.white : Color.gray;
    }
}