using UnityEngine;
using UnityEngine.EventSystems;

public class UiElementTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler, ISelectHandler
{
    public string text;

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

    void StartHover(Vector3 position)
    {
        TooltipManager.Instance.ShowTooltip(text, position);
    }
    void StopHover()
    {
        TooltipManager.Instance.HideTooltip();
    }
}
