using Assets.Zilon.Scripts.Services;

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

    [Inject] private readonly ISectorUiState _playerState;
    [Inject] private readonly ICommandManager<SectorCommandContext> _commandManager;
    [Inject] private readonly IInventoryState _inventoryState;
    [Inject] private readonly SpecialCommandManager _specialCommandManager;

    [Inject(Id = "use-self-command")] private readonly ICommand<SectorCommandContext> _useSelfCommand;
    [Inject(Id = "show-history-command")] private readonly ICommand<SectorCommandContext> _showHistoryCommand;

    public PropItemVm PropItemViewModel;
    public SectorCommandContextFactory SectorCommandContextFactory;

    public void OnPointerDown(PointerEventData eventData)
    {
        if ((lastClick + interval) > Time.time)
        {
            _inventoryState.SelectedProp = PropItemViewModel;

            var prop = PropItemViewModel.Prop;

            if (!(prop.Scheme.Sid == HISTORY_BOOK_SID))
            {
                var canUseProp = prop.Scheme.Use != null;
                if (canUseProp)
                {
                    UseProp();
                }
                else if (prop.Scheme.Equip != null)
                {
                    EquipProp(PropItemViewModel);
                }
            }
            else
            {
                ReadProp();
            }
        }

        lastClick = Time.time;
    }

    /// <summary>
    /// Метод выбирает слот по типу предмета. Выбор слота выполняется на основе команды на экипировку.
    /// 1. Перебираем все слоты персонажа.
    /// 2. Если экипировка возможна в текущий слот, то 3.
    /// 3. Выполняем команду на экипировку в текущий слот.
    /// </summary>
    private void EquipProp(IPropItemViewModel propItemViewModel)
    {
        _inventoryState.SelectedProp = propItemViewModel;

        var actor = _playerState.ActiveActor.Actor;
        var person = actor.Person;
        var personSlots = person.EquipmentCarrier.Slots;

        var sectorCommandContext = SectorCommandContextFactory.CreateContext();

        for (var slotIndex = 0; slotIndex < personSlots.Length; slotIndex++)
        {
            var equipCommand = _specialCommandManager.GetEquipCommand(slotIndex);
            if (equipCommand.CanExecute(sectorCommandContext))
            {
                _commandManager.Push(equipCommand);

                break;
            }
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
            _commandManager.Push(_showHistoryCommand);
        }
    }
}
