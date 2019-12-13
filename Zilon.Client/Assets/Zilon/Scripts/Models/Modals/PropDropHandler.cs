using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;

public class PropDropHandler : MonoBehaviour, IDropHandler
{
    private const string HISTORY_BOOK_SID = "history-book";

    [NotNull] [Inject] private readonly ICommandManager<SectorCommandContext> _commandManager;
    [NotNull] [Inject] private readonly ICommandManager<ActorModalCommandContext> _modalCommandManager;
    [NotNull] [Inject] private readonly IInventoryState _inventoryState;
    [NotNull] [Inject(Id = "use-self-command")] private readonly ICommand<SectorCommandContext> _useSelfCommand;
    [NotNull] [Inject(Id = "show-history-command")] private readonly ICommand<ActorModalCommandContext> _showHistoryCommand;

    public void OnDrop(PointerEventData eventData)
    {
        var droppedPropItem = eventData.pointerDrag?.GetComponent<PropItemVm>();
        _inventoryState.SelectedProp = droppedPropItem;

        var prop = droppedPropItem?.Prop;

        if (!(prop.Scheme.Sid == HISTORY_BOOK_SID))
        {
            var canUseProp = prop.Scheme.Use != null;
            if (canUseProp)
            {
                UseProp();
            }
        }
        else
        {
            ReadProp();
        }
    }

    private void UseProp()
    {
        _commandManager.Push(_useSelfCommand);
    }

    private void ReadProp()
    {
        if (_inventoryState.SelectedProp.Prop.Scheme.Sid == HISTORY_BOOK_SID)
        {
            _modalCommandManager.Push(_showHistoryCommand);
        }
    }
}
