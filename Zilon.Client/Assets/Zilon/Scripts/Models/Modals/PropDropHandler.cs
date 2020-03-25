using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Props;

public class PropDropHandler : MonoBehaviour, IDropHandler
{
    [NotNull] [Inject] private readonly ICommandManager _commandManager;
    [NotNull] [Inject] private readonly IInventoryState _inventoryState;
    [NotNull] [Inject(Id = "use-self-command")] private readonly ICommand _useSelfCommand;

    public void OnDrop(PointerEventData eventData)
    {
        var droppedPropItem = GetPropItemViewModel(eventData);
        _inventoryState.SelectedProp = droppedPropItem;

        var prop = GetPropFromViewModelSafe(droppedPropItem);

        var canUseProp = prop.Scheme.Use != null;
        if (canUseProp)
        {
            UseProp();
        }
    }

    private static IProp GetPropFromViewModelSafe(PropItemVm droppedPropItem)
    {
        if (droppedPropItem is null)
        {
            return null;
        }

        return droppedPropItem.Prop;
    }

    private static PropItemVm GetPropItemViewModel(PointerEventData eventData)
    {
        if (eventData.pointerDrag is null)
        {
            return null;
        }

        return eventData.pointerDrag.GetComponent<PropItemVm>();
    }

    private void UseProp()
    {
        _commandManager.Push(_useSelfCommand);
    }
}
