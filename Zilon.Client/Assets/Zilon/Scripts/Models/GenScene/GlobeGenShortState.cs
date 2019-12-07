using System;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class GlobeGenShortState : MonoBehaviour
{
    private DateTime? _globeCreatedTime;

    public GlobeKeeper GlobeKeeper;
    public Text Text;

    private void Update()
    {
        var globe = GlobeKeeper.Globe;
        if (globe == null)
        {
            Text.text = string.Empty;
            return;
        }

        if (_globeCreatedTime == null)
        {
            _globeCreatedTime = DateTime.Now;
        }

        var sb = new StringBuilder();

        sb.AppendLine($"Globe created {_globeCreatedTime}");

        sb.AppendLine($"ITERATION {globe.Iteration}");

        Text.text = sb.ToString();
    }
}
