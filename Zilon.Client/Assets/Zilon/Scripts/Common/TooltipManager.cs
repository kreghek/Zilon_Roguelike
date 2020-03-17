using Assets.Zilon.Scripts.Services;

using UnityEngine;

using Zenject;

public class TooltipManager : MonoBehaviour
{
    [Inject]
    private readonly UiSettingService _uiSettingService;

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

    public void ShowLocalizedTooltip(string key, Vector3 pos)
    {
        var currentLanguage = _uiSettingService.CurrentLanguage;
        var toolTipLocalText = StaticPhrases.GetValue(key, currentLanguage);

        if (tooltipText.text != toolTipLocalText)
            tooltipText.text = toolTipLocalText;

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
