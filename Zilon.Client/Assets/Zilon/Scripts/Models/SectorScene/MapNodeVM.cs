using System;
using UnityEngine;
using Zilon.Core.Tactics.Spatial;

public class MapNodeVM : MonoBehaviour {

    public HexNode Node { get; set; }

    public event EventHandler OnSelect;

    public void OnMouseDown()
    {
        OnSelect?.Invoke(this, new EventArgs());
    }

    public override string ToString()
    {
        if (Node == null)
        {
            return string.Empty;
        }

        return $"Id: {Node.Id} Position: ({Node.OffsetX}, {Node.OffsetY})";
    }
}
