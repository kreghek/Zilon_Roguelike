using System;

using UnityEngine;

using Zilon.Core.World;

public class MapLocation : MonoBehaviour
{
    public event EventHandler OnSelect;
    public SpriteRenderer Icon;

    public GlobeRegionNode Node { get; set; }

    public void Start()
    {
        if (Node.Scheme.Sid != "forest")
        {
            Icon.color = Color.red;
        }
    }

    public void OnMouseDown()
    {
        OnSelect(this, new EventArgs());
    }

    internal void SetAvailableState(bool state)
    {
        Icon.color = state ? Color.white : Color.gray;
    }
}