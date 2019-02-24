using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;

public class SectorGameCursor : MonoBehaviour
{
    public Sprite DefaultCursorSprite;
    public Sprite AttackCursorSprite;
    public Sprite CantMoveCursorSprite;
    public SpriteRenderer SpriteRenderer;

    [Inject] private readonly IPlayerState _playerState;
    [Inject(Id = "move-command")] private readonly ICommand _moveCommand;

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

        if (_playerState.HoverViewModel is IMapNodeViewModel nodeViewModel)
        {
            if (!_moveCommand.CanExecute())
            {
                SpriteRenderer.sprite = CantMoveCursorSprite;
            }
        }
        else if (_playerState.HoverViewModel is IActorViewModel actorViewModel)
        {
            SpriteRenderer.sprite = AttackCursorSprite;
        }
    }
}
