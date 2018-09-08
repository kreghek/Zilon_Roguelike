using System;

using UnityEngine;

using Zilon.Core.Client;
using Zilon.Core.Tactics;

public class ContainerVm : MonoBehaviour, IContainerViewModel
{
    public SpriteRenderer SpriteRenderer;
    public Sprite ClosedSprite;
    public Sprite OpenedSprite;


    public event EventHandler Selected;

    public IPropContainer Container { get; set; }

    public void Start()
    {
        SpriteRenderer.sprite = ClosedSprite;
        Container.IsOpenChanged += Container_IsOpenChanged;
    }

    public void OnMouseDown()
    {
        DoSelected();
    }

    private void DoSelected()
    {
        Selected?.Invoke(this, new EventArgs());
    }


    private void Container_IsOpenChanged(object sender, EventArgs e)
    {
        if (Container.IsOpened)
        {
            Debug.Log($"Сундук {sender} открыт");
            SpriteRenderer.sprite = OpenedSprite;
        }
        else
        {
            Debug.Log($"Сундук {sender} закрыт");
            SpriteRenderer.sprite = ClosedSprite;
        }
    }
}
