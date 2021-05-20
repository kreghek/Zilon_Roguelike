﻿using System.Linq;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Graphs;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

//TODO Сделать отдельные крипты для каждой кнопки, которые будут содежать обработчики.
//Тогда этот объект станет не нужным.
/// <summary>
/// Скрипт для обработки UI на глобальной карте.
/// </summary>
public class SectorUiHandler : MonoBehaviour
{
    [Inject] private readonly IPlayer _player;

    [Inject] private readonly ISectorUiState _playerState;

    [Inject] private readonly ICommandPool _commandPool;

    [Inject] private readonly IActorTaskControlSwitcher _actorTaskControlSwitcher;

    [Inject(Id = "next-turn-command")] private readonly ICommand _nextTurnCommand;

    [Inject(Id = "show-inventory-command")] private readonly ICommand _showInventoryCommand;

    [Inject(Id = "show-perks-command")] private readonly ICommand _showPersonModalCommand;

    [Inject(Id = "quit-request-command")] private readonly ICommand _quitRequestCommand;

    [Inject(Id = "quit-request-title-command")] private readonly ICommand _quitRequestTitleCommand;

    [Inject(Id = "open-container-command")] private readonly ICommand _openContainerCommand;


    [NotNull]
    [Inject(Id = "sector-transition-move-command")]
    private readonly ICommand _sectorTransitionMoveCommand;

    public Button NextTurnButton;
    public Button InventoryButton;
    public Button PersonButton;
    public Button SectorTransitionMoveButton;
    public Button OpenLootButton;

    public SectorVM SectorVM;

    public void Update()
    {
        HandleHotKeys();
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        var playerPersonIsNotInTransitionPool = _player.Globe.SectorNodes.Select(x => x.Sector).SelectMany(x => x.ActorManager.Items).Any(x => x.Person == _player.MainPerson);
        var playerPersonIsInTransitionPool = !playerPersonIsNotInTransitionPool;
        if (playerPersonIsInTransitionPool)
        {
            return;
        }

        if (NextTurnButton != null)
        {
            NextTurnButton.interactable = _nextTurnCommand.CanExecute().IsSuccess;
        }

        if (InventoryButton != null)
        {
            InventoryButton.interactable = _showInventoryCommand.CanExecute().IsSuccess;
        }

        if (PersonButton != null)
        {
            PersonButton.interactable = _showPersonModalCommand.CanExecute().IsSuccess;
        }

        if (SectorTransitionMoveButton != null)
        {
            if (_player.Globe is null || _player.MainPerson is null)
            {
                SectorTransitionMoveButton.interactable = false;
            }
            else
            {
                SectorTransitionMoveButton.interactable = _sectorTransitionMoveCommand.CanExecute().IsSuccess;
            }
        }

        if (OpenLootButton != null)
        {
            if (_player.Globe is null || _player.MainPerson is null)
            {
                OpenLootButton.gameObject.SetActive(false);
            }
            else
            {
                var canOpen = GetCanOpenLoot();
                OpenLootButton.gameObject.SetActive(canOpen);
            }
        }
    }

    private ISector CurrentSector => _player.SectorNode.Sector;

    private bool GetCanOpenLoot()
    {
        var personNode = _playerState.ActiveActor?.Actor?.Node;
        if (personNode is null)
        {
            return false;
        }

        var lootInNode = GetContainerInNode(personNode);
        if (lootInNode is null)
        {
            return false;
        }

        return true;
    }

    private IStaticObject GetContainerInNode(IGraphNode targetnNode)
    {
        var staticObjectManager = CurrentSector.StaticObjectManager;
        var containerStaticObjectInNode = staticObjectManager.Items
            .FirstOrDefault(x => x.Node == targetnNode && x.HasModule<IPropContainer>());
        return containerStaticObjectInNode;
    }

    private void HandleHotKeys()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            NextTurn();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ShowPersonModalButton_Handler();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            SectorTransitionMoveButton_Handler();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            OpenLoot_Handler();
        }
    }

    public void NextTurn()
    {
        if (_playerState.ActiveActor == null)
        {
            return;
        }

        _commandPool.Push(_nextTurnCommand);
    }

    public void ShowInventoryButton_Handler()
    {
        if (_playerState.ActiveActor == null)
        {
            return;
        }

        _commandPool.Push(_showInventoryCommand);
    }

    public void ShowPersonModalButton_Handler()
    {
        if (_playerState.ActiveActor == null)
        {
            return;
        }

        _commandPool.Push(_showPersonModalCommand);
    }

    public void SwitchAutoplay_Handler()
    {
        ActorTaskSourceControl targetControl;
        switch (_actorTaskControlSwitcher.CurrentControl)
        {
            case ActorTaskSourceControl.Human:
                targetControl = ActorTaskSourceControl.Bot;
                break;

            case ActorTaskSourceControl.Bot:
                targetControl = ActorTaskSourceControl.Human;
                break;

            default:
                Debug.LogError($"Unknown control value {_actorTaskControlSwitcher.CurrentControl}.");
                targetControl = ActorTaskSourceControl.Human;
                break;
        }
        _actorTaskControlSwitcher.Switch(targetControl);
    }

    public void ExitGame_Handler()
    {
        _commandPool.Push(_quitRequestCommand);
    }

    public void ExitTitle_Handler()
    {
        _commandPool.Push(_quitRequestTitleCommand);
    }

    public void SectorTransitionMoveButton_Handler()
    {
        // Защита от бага.
        // Пользователь может нажать T и выполнить переход.
        // Даже если мертв. Будет проявляться, когда пользователь вводит имя после смерти.
        if (_playerState.ActiveActor?.Actor.Person.GetModule<ISurvivalModule>().IsDead != false)
        {
            return;
        }

        _commandPool.Push(_sectorTransitionMoveCommand);
    }

    public void OpenLoot_Handler()
    {
        var personNode = _playerState.ActiveActor.Actor.Node;
        var lootInNode = GetContainerInNode(personNode);

        if (lootInNode != null)
        {
            var viewModel = GetLootViewModel(lootInNode);
            _playerState.SelectedViewModel = viewModel;
            _commandPool.Push(_openContainerCommand);
        }
    }

    private static ContainerVm GetLootViewModel(IStaticObject lootInNode)
    {
        var viewModels = FindObjectsOfType<ContainerVm>();
        foreach (var viewModel in viewModels)
        {
            if (viewModel.Container == lootInNode)
            {
                return viewModel;
            }
        }

        return null;
    }
}
