using System;

using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class CombatLogPanel : MonoBehaviour
{
    public Text LogText;

    [Inject] private readonly ILogService _logService;

    public void Start()
    {
        LogText.text = string.Empty;
        _logService.LogChanged += LogService_LogChanged;
    }

    public void OnDestroy()
    {
        _logService.LogChanged -= LogService_LogChanged;
    }

    private void LogService_LogChanged(object sender, LogChangedEventArgs e)
    {
        LogText.text = e.Message + Environment.NewLine + LogText.text;

        if (LogText.text.Length > 1000)
        {
            LogText.text = LogText.text.Substring(0, 1000);
        }
    }
}
