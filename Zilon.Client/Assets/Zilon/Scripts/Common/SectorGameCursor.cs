﻿using System;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;

public class SectorGameCursor : MonoBehaviour
{
    public Sprite DefaultCursorSprite;
    public Sprite AttackCursorSprite;
    public Sprite InteractiveCursorSprite;
    public Sprite CantMoveCursorSprite;
    public SpriteRenderer SpriteRenderer;
    private TaskScheduler _taskScheduler;

    [Inject] private readonly ISectorUiState _playerState;
    [Inject(Id = "move-command")] private readonly ICommand _moveCommand;
    [Inject(Id = "attack-command")] private readonly ICommand _attackCommand;

    public void Start()
    {
        _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        Cursor.visible = false;
        SpriteRenderer.sprite = DefaultCursorSprite;

        _playerState.HoverChanged += PlayerState_HoverChanged;
    }

    private async void PlayerState_HoverChanged(object sender, EventArgs e)
    {
        if (_playerState.HoverViewModel == null)
        {
            return;
        }

        await Task.Factory.StartNew(() =>
        {
            try
            {
                HoverChangedHandlerInner();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
            }
        }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
    }

    private void HoverChangedHandlerInner()
    {
        SpriteRenderer.sprite = DefaultCursorSprite;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (_playerState.HoverViewModel is IContainerViewModel)
        {
            SpriteRenderer.sprite = InteractiveCursorSprite;
        }
        else if (_playerState.HoverViewModel is IMapNodeViewModel)
        {
            if (!_moveCommand.CanExecute().IsSuccess)
            {
                SpriteRenderer.sprite = CantMoveCursorSprite;
            }
        }
        else if (_playerState.HoverViewModel is IActorViewModel)
        {
            if (_attackCommand.CanExecute().IsSuccess)
            {
                SpriteRenderer.sprite = AttackCursorSprite;
            }
            else
            {
                if (_playerState.HoverViewModel != _playerState.ActiveActor)
                {
                    SpriteRenderer.sprite = CantMoveCursorSprite;
                }
            }
        }
    }

    public void OnDestroy()
    {
        Cursor.visible = true;
        _playerState.HoverChanged -= PlayerState_HoverChanged;
    }

    public void Update()
    {
        var cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.localPosition = cursorPosition + new Vector3(0, 0, 11);
    }
}
