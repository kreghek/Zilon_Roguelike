using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
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

    [Inject(Id = "next-turn-command")] private readonly ICommand _nextTurnCommand;

    [Inject(Id = "show-inventory-command")] private readonly ICommand _showInventoryCommand;

    [Inject(Id = "show-perks-command")] private readonly ICommand _showPersonModalCommand;

    [Inject(Id = "quit-request-command")] private readonly ICommand _quitRequestCommand;


    [NotNull]
    [Inject(Id = "sector-transition-move-command")]
    private readonly ICommand _sectorTransitionMoveCommand;

    public Button NextTurnButton;
    public Button InventoryButton;
    public Button PersonButton;
    public Button SectorTransitionMoveButton;
    public Button CityQuickExitButton;

    public void FixedUpdate()
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
    }

    public void Update()
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

    public void SectorTransitionMoveButton_Handler()
    {
        _clientCommandExecutor.Push(_sectorTransitionMoveCommand);
    }

    public void CityQuickExit_Handler()
    {
        // Это быстрое решение.
        // Тупо загружаем глобальную карту.
        SceneManager.LoadScene("globe");
    }
}
