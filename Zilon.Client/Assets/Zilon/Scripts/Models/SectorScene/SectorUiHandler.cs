using System.Linq;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Graphs;
using Zilon.Core.Tactics;

//TODO Сделать отдельные крипты для каждой кнопки, которые будут содежать обработчики.
//Тогда этот объект станет не нужным.
/// <summary>
/// Скрипт для обработки UI на глобальной карте.
/// </summary>
public class SectorUiHandler : MonoBehaviour
{
    [Inject] private readonly ISectorUiState _playerState;

    [Inject] private readonly ISectorManager _sectorManager;

    [Inject] private readonly ICommandManager _clientCommandExecutor;

    [Inject] private readonly IPropContainerManager _propContainerManager;

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
    public Button CityQuickExitButton;
    public Button OpenLootButton;

    public GameObject SleepShadow;

    public void Update()
    {
        HandleHotKeys();
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        if (NextTurnButton != null)
        {
            NextTurnButton.interactable = _nextTurnCommand.CanExecute();
        }

        if (InventoryButton != null)
        {
            InventoryButton.interactable = _showInventoryCommand.CanExecute();
        }

        if (PersonButton != null)
        {
            PersonButton.interactable = _showPersonModalCommand.CanExecute();
        }

        if (SectorTransitionMoveButton != null)
        {
            SectorTransitionMoveButton.interactable = _sectorTransitionMoveCommand.CanExecute();
        }

        if (CityQuickExitButton != null)
        {
            // Это быстрое решение.
            // Проверяем, если это город, то делаем кнопку выхода видимой.
            var isInCity = _sectorManager.CurrentSector?.Scheme.Sid == "city";
            CityQuickExitButton.gameObject.SetActive(isInCity);
        }

        if (OpenLootButton != null)
        {
            var canOpen = GetCanOpenLoot();
            OpenLootButton.gameObject.SetActive(canOpen);
        }
    }

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

    private IPropContainer GetContainerInNode(IGraphNode targetnNode)
    {
        var containerInNode = _propContainerManager.Items.FirstOrDefault(x => x.Node == targetnNode);
        return containerInNode;
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

        // Отключено, потому что сейчас нет выхода на глобальную карту.
        if (Input.GetKeyDown(KeyCode.G))
        {
            CityQuickExit_Handler();
        }
    }

    public void NextTurn()
    {
        if (_playerState.ActiveActor == null)
        {
            return;
        }

        _clientCommandExecutor.Push(_nextTurnCommand);
    }

    public void Sleep_Handler()
    {
        if (_playerState.ActiveActor == null)
        {
            return;
        }

        for (var i = 0; i < 30; i++)
        {
            NextTurn();
        }
    }

    public void ShowInventoryButton_Handler()
    {
        if (_playerState.ActiveActor == null)
        {
            return;
        }

        _clientCommandExecutor.Push(_showInventoryCommand);
    }

    public void ShowPersonModalButton_Handler()
    {
        if (_playerState.ActiveActor == null)
        {
            return;
        }

        _clientCommandExecutor.Push(_showPersonModalCommand);
    }

    public void ExitGame_Handler()
    {
        _clientCommandExecutor.Push(_quitRequestCommand);
    }

    public void ExitTitle_Handler()
    {
        _clientCommandExecutor.Push(_quitRequestTitleCommand);
    }

    public void SectorTransitionMoveButton_Handler()
    {
        // Защита от бага.
        // Пользователь может нажать T и выполнить переход.
        // Даже если мертв. Будет проявляться, когда пользователь вводит имя после смерти.
        if (_playerState.ActiveActor.Actor.Person.Survival.IsDead != false)
        {
            return;
        }

        _clientCommandExecutor.Push(_sectorTransitionMoveCommand);
    }

    public void CityQuickExit_Handler()
    {
        // Защита от бага.
        // Пользователь может нажать Q и выйти из сектора на глобальную карту.
        // Даже если мертв. Будет проявляться, когда пользователь вводит имя после смерти.
        if (_playerState.ActiveActor.Actor.Person.Survival.IsDead != false)
        {
            return;
        }

        // Это быстрое решение.
        // Тупо загружаем глобальную карту.
        SceneManager.LoadScene("globe");
    }

    public void OpenLoot_Handler()
    {
        var personNode = _playerState.ActiveActor.Actor.Node;
        var lootInNode = GetContainerInNode(personNode);

        if (lootInNode != null)
        {
            var viewModel = GetLootViewModel(lootInNode);
            _playerState.SelectedViewModel = viewModel;
            _clientCommandExecutor.Push(_openContainerCommand);
        }
    }

    private static ContainerVm GetLootViewModel(IPropContainer lootInNode)
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
