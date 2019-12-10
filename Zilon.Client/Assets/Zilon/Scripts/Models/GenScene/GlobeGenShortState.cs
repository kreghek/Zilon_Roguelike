using System;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.World;

public class GlobeGenShortState : MonoBehaviour
{
    [Inject]
    private readonly IGlobeManager _globeManager;

    private DateTime? _globeCreatedTime;

    public Text Text;

    private void Update()
    {
        var globe = _globeManager.Globe;
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
