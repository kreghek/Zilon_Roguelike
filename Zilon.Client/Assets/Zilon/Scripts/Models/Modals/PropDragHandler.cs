using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

public class PropDragHandler : UIBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private DraggedPropItem _draggedPropItem;

    public PropItemVm PropItemViewModel;
    public DraggedPropItem DraggedPropItemPrefab;

    [NotNull] [Inject] private readonly DiContainer _diContainer;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Этот блок на свякий случай.
        // Прецедентов не было, но удаление может сломаться.
        if (_draggedPropItem != null)
        {
            Debug.LogError("Была ошибка при удалении.");
            Destroy(_draggedPropItem.gameObject);
        }

        // Эта проверка будет до задачи экипировки через dnd
        if (PropItemViewModel.Prop.Scheme.Use == null)
        {
            return;
        }

        var parentCanvas = FindObjectOfType<Canvas>();

        var draggedPropItemObj = _diContainer.InstantiatePrefab(DraggedPropItemPrefab, parentCanvas.transform);

        _draggedPropItem = draggedPropItemObj.GetComponent<DraggedPropItem>();
        _draggedPropItem.Init(PropItemViewModel);

        PropItemViewModel.SetDraggingState(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggedPropItem != null)
        {
            Destroy(_draggedPropItem.gameObject);
        }

        PropItemViewModel.SetDraggingState(false);
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
}
