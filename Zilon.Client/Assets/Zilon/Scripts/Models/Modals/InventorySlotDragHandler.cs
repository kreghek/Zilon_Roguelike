using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

//TODO Объединить *DragHandler
// PropDragHandler и InventorySlotDragHandler.
// У них много общего. И достаточно сложная логика.
public class InventorySlotDragHandler : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Inject]
    private DiContainer _diContainer;

    public DraggedPropItem DraggedPropItemPrefab;
    public InventorySlotVm InventorySlotViewModel;

    private DraggedPropItem _draggedPropItem;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Этот блок на свякий случай.
        // Прецедентов не было, но удаление может сломаться.
        if (_draggedPropItem != null)
        {
            Debug.LogError("Была ошибка при удалении.");
            Destroy(_draggedPropItem.gameObject);
        }

        var parentCanvas = FindObjectOfType<Canvas>();

        var draggedPropItemObj = _diContainer.InstantiatePrefab(DraggedPropItemPrefab, parentCanvas.transform);

        _draggedPropItem = draggedPropItemObj.GetComponent<DraggedPropItem>();
        _draggedPropItem.Init(InventorySlotViewModel);

        InventorySlotViewModel.SetDraggingState(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_draggedPropItem != null)
        {
            // Это преобразование нужно для того, чтобы объект перетаскиваемого предмета корректно отрисовывался под курсором.
            // Если сделать просто _draggedPropItem.transform.position = Input.mousePosition, как это показывают в мануалах,
            // то объект вообще будет иметь координаты, далёкие от канваса.
            var parentRect = (RectTransform)_draggedPropItem.transform.parent;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, Camera.main, out var posInParent);

            _draggedPropItem.transform.localPosition = posInParent;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggedPropItem != null)
        {
            Destroy(_draggedPropItem.gameObject);
        }

        InventorySlotViewModel.SetDraggingState(false);
    }
}
