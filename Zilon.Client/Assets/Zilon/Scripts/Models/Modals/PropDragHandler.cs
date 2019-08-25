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

        PropItemViewModel.IconImage.color = new Color(1, 1, 1, 0.5f);
        PropItemViewModel.SelectAsDrag = true;
        PropItemViewModel.Click_Handler();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggedPropItem != null)
        {
            Destroy(_draggedPropItem.gameObject);
        }

        PropItemViewModel.IconImage.color = new Color(1, 1, 1, 1);
        PropItemViewModel.SelectAsDrag = false;
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
