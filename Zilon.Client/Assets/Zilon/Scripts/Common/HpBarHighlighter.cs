using UnityEngine;
using UnityEngine.UI;

public sealed class HpBarHighlighter : MonoBehaviour
{
    private float _counter;

    public Outline Outline;

    public void StartHighlighting()
    {
        _counter = 0;
        Outline.enabled = true;
        enabled = true;

    }

    public void FixedUpdate()
    {
        _counter += Time.deltaTime;

        Outline.effectDistance = Vector2.one * Mathf.PingPong(_counter * 2, 1) * 10f;

        if (_counter > 2)
        {
            Outline.enabled = false;
            enabled = false;
        }
    }
}

