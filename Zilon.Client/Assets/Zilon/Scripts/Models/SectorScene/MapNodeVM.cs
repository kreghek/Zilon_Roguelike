using System;
using UnityEngine;
using Zilon.Core.Tactics.Map;

public class MapNodeVM : MonoBehaviour {

    public MapNode Node { get; set; }

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

        return $"Id: {Node.Id} Position: {Node.Position}";
    }
}
