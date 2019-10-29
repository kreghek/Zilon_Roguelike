using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public bool IsActive
    {
        get
        {
            return gameObject.activeSelf;
        }
    }
    //public CanvasGroup tooltip;
    public UnityEngine.UI.Text tooltipText;

    void Awake()
    {
        instance = this;
        HideTooltip();
    }

    public void ShowTooltip(string text, Vector3 pos)
    {
        if (tooltipText.text != text)
            tooltipText.text = text;

        var parentRect = (RectTransform)transform.parent;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, pos, Camera.main, out var posInParent);

        transform.localPosition = posInParent;

        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    // Standard Singleton Access 
    private static TooltipManager instance;
    public static TooltipManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<TooltipManager>();
            return instance;
        }
    }
}
