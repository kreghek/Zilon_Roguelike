using System;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

public class ContainerVm : StaticObjectViewModel
{
    private TaskScheduler _taskScheduler;
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
            if (_propContainer is null)
            {
                throw new InvalidOperationException();
            }
        }
    }

    public override void Start()
    {
        _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        base.Start();

        var isOpened = Container.GetModule<IPropContainer>().IsOpened;
        SpriteRenderer.sprite = isOpened ? OpenedSprite : ClosedSprite;

        _propContainer.Opened += Container_Opened;
    }

    private void OnDestroy()
    {
        _propContainer.Opened -= Container_Opened;
    }

    private async void Container_Opened(object sender, EventArgs e)
    {
        await Task.Factory.StartNew(() =>
        {
            SpriteRenderer.sprite = OpenedSprite;
        }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
    }
}