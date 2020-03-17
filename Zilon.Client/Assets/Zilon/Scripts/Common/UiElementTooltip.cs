using UnityEngine;
using UnityEngine.EventSystems;

public class UiElementTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler, ISelectHandler
{
    public string text;
    public bool TextIsKey;

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartHover(new Vector3(eventData.position.x, eventData.position.y - 18f, 0f));
    }
    public void OnSelect(BaseEventData eventData)
    {
        StartHover(transform.position);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        StopHover();
    }
    public void OnDeselect(BaseEventData eventData)
    {
        StopHover();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Correctness", "UNT0008:Null propagation on Unity objects",
        Justification = "TooltipManager может вернуть null, если на сцену не разместили нужный компонент")]
    void StartHover(Vector3 position)
    {
        if (!TextIsKey)
        {
            TooltipManager.Instance?.ShowTooltip(text, position);
        }
        else
        {
            TooltipManager.Instance?.ShowLocalizedTooltip(text, position);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Correctness", "UNT0008:Null propagation on Unity objects",
        Justification = "TooltipManager может вернуть null, если на сцену не разместили нужный компонент")]
    void StopHover()
    {
        TooltipManager.Instance?.HideTooltip();
    }
}
