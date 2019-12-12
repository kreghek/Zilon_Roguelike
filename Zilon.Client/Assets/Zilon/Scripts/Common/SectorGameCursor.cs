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
    public SectorCommandContextFactory SectorCommandContextFactory;

    [Inject] private readonly ISectorUiState _playerState;
    [Inject(Id = "move-command")] private readonly ICommand<SectorCommandContext> _moveCommand;
    [Inject(Id = "attack-command")] private readonly ICommand<SectorCommandContext> _attackCommand;

    public void Start()
    {
        Cursor.visible = false;
        SpriteRenderer.sprite = DefaultCursorSprite;

        _playerState.HoverChanged += PlayerState_HoverChanged;
    }

    private void PlayerState_HoverChanged(object sender, System.EventArgs e)
    {
        SpriteRenderer.sprite = DefaultCursorSprite;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        var sectorCommandContext = SectorCommandContextFactory.Create();

        if (_playerState.HoverViewModel is IContainerViewModel)
        {
            SpriteRenderer.sprite = InteractiveCursorSprite;
        }
        else if (_playerState.HoverViewModel is IMapNodeViewModel)
        {
            if (!_moveCommand.CanExecute(sectorCommandContext))
            {
                SpriteRenderer.sprite = CantMoveCursorSprite;
            }
        }
        else if (_playerState.HoverViewModel is IActorViewModel)
        {
            if (_attackCommand.CanExecute(sectorCommandContext))
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
