using UnityEngine;
using UnityEngine.EventSystems;

public class PropBeginDragHandler : UIBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private DraggedPropItem _draggedPropItem;

    public PropItemVm PropItemViewModel;
    public DraggedPropItem DraggedPropItemPrefab;

    public void OnBeginDrag(PointerEventData eventData)
    {
        var parentCanvas = FindObjectOfType<Canvas>();

        _draggedPropItem = Instantiate(DraggedPropItemPrefab, parentCanvas.transform);
        _draggedPropItem.Init(PropItemViewModel.Prop);

        PropItemViewModel.IconImage.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggedPropItem != null)
        {
            Destroy(_draggedPropItem.gameObject);
        }

        PropItemViewModel.IconImage.color = new Color(1, 1, 1, 1);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_draggedPropItem != null)
        {
            var parentRect = (RectTransform)_draggedPropItem.transform.parent;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, Camera.main, out var posInParent);

            _draggedPropItem.transform.localPosition = posInParent;
        }
    }
}
