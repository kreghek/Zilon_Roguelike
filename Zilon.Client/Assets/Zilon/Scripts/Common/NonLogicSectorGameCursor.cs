using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;

public class NonLogicSectorGameCursor : MonoBehaviour
{
    public Sprite DefaultCursorSprite;

    public SpriteRenderer SpriteRenderer;

    public void Start()
    {
        Cursor.visible = false;
        SpriteRenderer.sprite = DefaultCursorSprite;
    }

    public void OnDestroy()
    {
        Cursor.visible = true;
    }

    public void Update()
    {
        var cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.localPosition = cursorPosition + new Vector3(0, 0, 11);
    }
}
