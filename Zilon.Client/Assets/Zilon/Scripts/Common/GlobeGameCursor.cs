using UnityEngine;

public class GlobeGameCursor : MonoBehaviour
{
    public Sprite DefaultCursorSprite;
    public SpriteRenderer SpriteRenderer;

    public void Start()
    {
        Cursor.visible = false;
        SpriteRenderer.sprite = DefaultCursorSprite;
    }


    public void Update()
    {
        var cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.localPosition = cursorPosition + new Vector3(0, 0, 11);
    }

    public void OnDestroy()
    {
        Cursor.visible = true;
    }
}
