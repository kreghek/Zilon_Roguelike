using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;

public class DoubleClickPropHandler : MonoBehaviour, IPointerDownHandler
{
    float lastClick = 0f;
    float interval = 0.4f;

    private const string HISTORY_BOOK_SID = "history-book";

    [NotNull] [Inject] private readonly ICommandManager _commandManager;
    [NotNull] [Inject] private readonly IInventoryState _inventoryState;
    [NotNull] [Inject(Id = "use-self-command")] private readonly ICommand _useSelfCommand;
    [NotNull] [Inject(Id = "show-history-command")] private readonly ICommand _showHistoryCommand;

    public PropItemVm PropItemViewModel;

    public void OnPointerDown(PointerEventData eventData)
    {
        if ((lastClick + interval) > Time.time)
        {
            var prop = PropItemViewModel?.Prop;

            if (!(prop.Scheme.Sid == HISTORY_BOOK_SID))
            {
                UseProp();
            }
            else
            {
                ReadProp();
            }
        }

        lastClick = Time.time;
    }

    private void UseProp()
    {
        _commandManager.Push(_useSelfCommand);
    }

    private void ReadProp()
    {
        if (_inventoryState.SelectedProp.Prop.Scheme.Sid == HISTORY_BOOK_SID)
        {
            _commandManager.Push(_showHistoryCommand);
        }
    }
}
