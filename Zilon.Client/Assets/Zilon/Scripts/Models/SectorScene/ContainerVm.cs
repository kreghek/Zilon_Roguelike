﻿using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Zilon.Core.Client;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

public class ContainerVm : MonoBehaviour, IContainerViewModel
{
    private IPropContainer _propContainer;

    public SpriteRenderer SpriteRenderer;
    public Sprite ClosedSprite;
    public Sprite OpenedSprite;
    private IStaticObject _container;

    public event EventHandler Selected;
    public event EventHandler MouseEnter;

    public IStaticObject Container
    {
        get => _container; set
        {
            _container = value;
            _propContainer = value.GetModule<IPropContainer>();
        }
    }

    public void Start()
    {
        SpriteRenderer.sprite = ClosedSprite;

        _propContainer.Opened += Container_Opened;

        var hexNode = (HexNode)Container.Node;
        //TODO -0.26 вынести в отдельную константу или вообще сервис.
        //https://answers.unity.com/questions/598492/how-do-you-set-an-order-for-2d-colliders-that-over.html
        // Статья, в которой подтверждается, что коллайдеры, расположенные на одной z-координате,
        // срабатывают в произвольном порядке.
        // Поэтому сундуки рендерятся ближе к камере и поднимают свой коллайдер.
        transform.position = new Vector3(transform.position.x, transform.position.y, hexNode.OffsetCoords.Y - 0.26f);
    }

    private void OnDestroy()
    {
        _propContainer.Opened -= Container_Opened;
    }

    public void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        DoSelected();
    }

    public void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        DoMouseEnter();
    }

    private void DoSelected()
    {
        Selected?.Invoke(this, new EventArgs());
    }

    private void DoMouseEnter()
    {
        MouseEnter?.Invoke(this, new EventArgs());
    }


    private void Container_Opened(object sender, EventArgs e)
    {
        SpriteRenderer.sprite = OpenedSprite;
    }
}