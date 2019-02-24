using UnityEngine;

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
    }


    public void Update()
    {
        var cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.localPosition = cursorPosition + new Vector3(0, 0, 11);
    }

    public void FixedUpdate()
    {
        SpriteRenderer.sprite = DefaultCursorSprite;

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
                SpriteRenderer.sprite = CantMoveCursorSprite;
            }
        }
        else if (_playerState.HoverViewModel is IContainerViewModel)
        {
            SpriteRenderer.sprite = InteractiveCursorSprite;
        }
    }
}
