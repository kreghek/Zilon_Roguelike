using System;

using UnityEngine;

using Zilon.Core.Tactics;

public class ContainerVm : StaticObjectViewModel
{
    private IPropContainer _propContainer;

    public SpriteRenderer SpriteRenderer;
    public Sprite ClosedSprite;
    public Sprite OpenedSprite;

    public override IStaticObject StaticObject
    {
        get => base.StaticObject;
        set
        {
            base.StaticObject = value;
            _propContainer = value.GetModule<IPropContainer>();
        }
    }

    public override void Start()
    {
        base.Start();

        SpriteRenderer.sprite = Container.GetModule<IPropContainer>().IsOpened ? OpenedSprite : ClosedSprite;

        _propContainer.Opened += Container_Opened;

    }

    private void OnDestroy()
    {
        _propContainer.Opened -= Container_Opened;
    }

    private void Container_Opened(object sender, EventArgs e)
    {
        SpriteRenderer.sprite = OpenedSprite;
    }
}