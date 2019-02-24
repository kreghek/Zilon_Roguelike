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

    [Inject] private readonly IPlayerState _playerState;
    [Inject(Id = "move-command")] private readonly ICommand _moveCommand;
    [Inject(Id = "attack-command")] private readonly ICommand _attackCommand;

    public void Start()
    {
        Cursor.visible = false;
        _playerState.HoverChanged += PlayerState_HoverChanged;
    }

    private void PlayerState_HoverChanged(object sender, System.EventArgs e)
    {
        SpriteRenderer.sprite = DefaultCursorSprite;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (_playerState.HoverViewModel is IMapNodeViewModel)
        {
            if (!_moveCommand.CanExecute())
            {
                SpriteRenderer.sprite = CantMoveCursorSprite;
            }
        }
        else if (_playerState.HoverViewModel is IActorViewModel)
        {
            if (_attackCommand.CanExecute())
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
        else if (_playerState.HoverViewModel is IContainerViewModel)
        {
            SpriteRenderer.sprite = InteractiveCursorSprite;
        }
    }

    public void OnDestroy()
    {
        _playerState.HoverChanged -= PlayerState_HoverChanged;
    }

    public void Update()
    {
        var cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.localPosition = cursorPosition + new Vector3(0, 0, 11);
    }
}
