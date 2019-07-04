using UnityEngine;
using UnityEngine.UI;

public sealed class HpBarHighlighter : MonoBehaviour
{
    private const float HIGHLIGHT_POWER_CRITICAL = 20f;
    private const float HIGHLIGHT_SPEED_CRITICAL = 10f;
    private const float HIGHLIGHT_POWER_WARNING = 5f;
    private const float HIGHLIGHT_SPEED_WARNING = 5f;
    private const float HGHTLIGHT_LIFETIME_SECONDS = 1.5f;

    private float _counter;

    private HighLightLevel _currentLevel;

    public Outline Outline;

    public void StartHighlighting(HighLightLevel level)
    {
        _counter = 0;
        Outline.enabled = true;
        enabled = true;
        _currentLevel = level;
    }

    public void FixedUpdate()
    {
        _counter += Time.deltaTime;

        var highlightSpeed = 0f;
        var highlightPower = 0f;

        switch (_currentLevel)
        {
            case HighLightLevel.Warning:
                highlightPower = HIGHLIGHT_POWER_WARNING;
                highlightSpeed = HIGHLIGHT_SPEED_WARNING;
                break;

            case HighLightLevel.Critical:
                highlightPower = HIGHLIGHT_POWER_CRITICAL;
                highlightSpeed = HIGHLIGHT_SPEED_CRITICAL;
                break;
        }

        Outline.effectDistance = Vector2.one * Mathf.PingPong(_counter * highlightSpeed, 1) * highlightPower;

        if (_counter > HGHTLIGHT_LIFETIME_SECONDS)
        {
            Outline.enabled = false;
            enabled = false;
        }
    }

    public enum HighLightLevel
    {
        NoHighlight,
        Warning,
        Critical
    }
}

