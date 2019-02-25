using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Zilon.Core.Client;
using Zilon.Core.Tactics;

public class TraderViewModel : MonoBehaviour, ITraderViewModel
{
    public SpriteRenderer SpriteRenderer;

    public event EventHandler Selected;

    public ITrader Trader { get; set; }

    public void Start()
    {
        // выбрать визуализацию торговца
        var visualSprite = Resources.Load<Sprite>($"Traders/trader1");
        SpriteRenderer.sprite = visualSprite;
    }

    public void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        DoSelected();
    }

    private void DoSelected()
    {
        Selected?.Invoke(this, new EventArgs());
    }
}
