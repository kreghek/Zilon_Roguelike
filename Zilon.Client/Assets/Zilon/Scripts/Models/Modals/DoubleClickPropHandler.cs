﻿using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Commands;
using Zilon.Core.PersonModules;

public class DoubleClickPropHandler : MonoBehaviour, IPointerDownHandler
{
    private float _lastClick = 0f;
    private const float INTERVAL = 0.4f;

    [Inject] private readonly ISectorUiState _playerState;
    [Inject] private readonly ICommandPool _commandPool;
    [Inject] private readonly IInventoryState _inventoryState;
    [Inject] private readonly SpecialCommandManager _specialCommandManager;
    [Inject] private readonly IAnimationBlockerService _animationBlockerService;

    [Inject(Id = "use-self-command")] private readonly ICommand _useSelfCommand;

    public PropItemVm PropItemViewModel;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_animationBlockerService.HasBlockers)
        {
            return;
        }

        if ((_lastClick + INTERVAL) > Time.time)
        {
            _inventoryState.SelectedProp = PropItemViewModel;

            var prop = PropItemViewModel.Prop;

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

        _lastClick = Time.time;
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
        var personSlots = person.GetModule<IEquipmentModule>().Slots;

        for (var slotIndex = 0; slotIndex < personSlots.Length; slotIndex++)
        {
            var equipCommand = _specialCommandManager.GetEquipCommand(slotIndex);
            if (equipCommand.CanExecute().IsSuccess)
            {
                _commandPool.Push(equipCommand);

                break;
            }
        }
    }

    private void UseProp()
    {
        _commandPool.Push(_useSelfCommand);
    }
}
