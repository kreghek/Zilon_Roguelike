using UnityEngine;

public class GlobeGameCursor : MonoBehaviour
{
    public Sprite DefaultCursorSprite;
    public Sprite AttackCursorSprite;
    public Sprite InteractiveCursorSprite;
    public Sprite CantMoveCursorSprite;
    public SpriteRenderer SpriteRenderer;

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
    }
}
