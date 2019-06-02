using System;

using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class CombatLogPanel : MonoBehaviour
{
    private const float SHOW_DURATION = 2;
    private const float DELAY_DURATION = SHOW_DURATION + 2;
    private float _counter = 0;

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

    public void FixedUpdate()
    {
        if (_counter <= SHOW_DURATION)
        {
            _counter += Time.deltaTime;
            LogText.color = new Color(LogText.color.r, LogText.color.g, LogText.color.b, 1);
        }
        else if (SHOW_DURATION <= _counter && _counter <= DELAY_DURATION)
        {
            _counter += Time.deltaTime;
            LogText.color = Color.Lerp(LogText.color, new Color(LogText.color.r, LogText.color.g, LogText.color.b, 0), Time.deltaTime * 0.5f);
        }
        else
        {
            LogText.color = new Color(LogText.color.r, LogText.color.g, LogText.color.b, 0);
        }
    }

    private void LogService_LogChanged(object sender, LogChangedEventArgs e)
    {
        _counter = 0;
        LogText.text = e.Message + Environment.NewLine + LogText.text;

        if (LogText.text.Length > 1000)
        {
            LogText.text = LogText.text.Substring(0, 1000);
        }
    }
}
