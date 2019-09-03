using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Commands;
using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

public class KeyboardMoveController : MonoBehaviour
{
    /// <summary>
    /// Интервал в секундах. Должен быть равен времени, пока персонаж
    /// визуально перемещается между узлами.
    /// </summary>
    private const float MOVE_COMMAND_INTERVAL = 0.3f;

    public SectorVM SectorViewModel;

    private float _moveCommandCounter = 0;

    private readonly Dictionary<StepDirection, int> _stepDirectionIndexes = new Dictionary<StepDirection, int> {
        { StepDirection.Left, 0 },
        { StepDirection.LeftTop, 1 },
        { StepDirection.RightTop, 2 },
        { StepDirection.Right, 3 },
        { StepDirection.RightBottom, 4 },
        { StepDirection.LeftBottom, 5 }
    };

    private readonly Dictionary<KeyCode, StepDirection> _directionMap = new Dictionary<KeyCode, StepDirection> {
        { KeyCode.Keypad4, StepDirection.Left },
        { KeyCode.Keypad7, StepDirection.LeftTop },
        { KeyCode.Keypad9, StepDirection.RightTop },
        { KeyCode.Keypad6, StepDirection.Right },
        { KeyCode.Keypad3, StepDirection.RightBottom },
        { KeyCode.Keypad1, StepDirection.LeftBottom },

        { KeyCode.A, StepDirection.Left },
        { KeyCode.Q, StepDirection.LeftTop },
        { KeyCode.W, StepDirection.RightTop },
        { KeyCode.S, StepDirection.Right },
        { KeyCode.X, StepDirection.RightBottom },
        { KeyCode.Z, StepDirection.LeftBottom }
    };

    [Inject]
    private readonly ISectorUiState _sectorUiState;

    [Inject]
    private readonly ISectorManager _sectorManager;

    [Inject]
    private readonly ICommandBlockerService _commandBlockerService;

    [NotNull]
    [Inject]
    private readonly ICommandManager _clientCommandExecutor;

    [NotNull]
    [Inject(Id = "move-command")]
    private readonly ICommand _moveCommand;

    // Update is called once per frame
    void Update()
    {
        // Эта проверка нужна для избежания ошибки при выполнении команды.
        // Предположительно, ошибка происходит, потому что _commandBlockerService.HasBlockers
        // ещё не выставлен, а уже отправлен второй запрос на команду.
        //TODO Убедиться в достоверности утверждения выше.

        _moveCommandCounter += Time.deltaTime;
        if (_moveCommandCounter <= MOVE_COMMAND_INTERVAL)
        {
            return;
        }

        if (!_commandBlockerService.HasBlockers)
        {
            var direction = GetDirectionByKeyboard();

            // Сброс интервала только когда пользователь совершил перемещение
            // То есть нажал на стрелки.
            _moveCommandCounter = 0;

            if (direction == StepDirection.Undefined)
            {
                return;
            }

            var actorHexNode = _sectorUiState.ActiveActor.Actor.Node as HexNode;

            var targetNode = GetTargetNode(actorHexNode, direction);

            if (actorHexNode == targetNode)
            {
                //TODO Нужно обосновать наличие этого выхода.
                return;
            }

            var targetNodeViewModel = SectorViewModel.NodeViewModels.SingleOrDefault(x => ReferenceEquals(x.Node, targetNode));

            _sectorUiState.SelectedViewModel = targetNodeViewModel;

            if (targetNodeViewModel != null)
            {
                _clientCommandExecutor.Push(_moveCommand);
            }
        }
    }

    private StepDirection GetDirectionByKeyboard()
    {
        var pressedKeyCode = DetectPressedKeyOrButton();
        if (_directionMap.TryGetValue(pressedKeyCode, out var stepDirection))
        {
            return stepDirection;
        }

        return StepDirection.Undefined;
    }

    private static KeyCode DetectPressedKeyOrButton()
    {
        var allKeyCodes = Enum.GetValues(typeof(KeyCode));
        foreach (KeyCode keyCode in allKeyCodes)
        {
            if (Input.GetKey(keyCode))
            {
                return keyCode;
            }
        }

        return KeyCode.None;
    }

    private IMapNode GetTargetNode(HexNode actorHexNode, StepDirection direction)
    {
        if (direction == StepDirection.Undefined)
        {
            throw new ArgumentException("Не определено направление.", nameof(direction));
        }

        var neighborNodes = _sectorManager.CurrentSector.Map.GetNext(actorHexNode).OfType<HexNode>();
        var directions = HexHelper.GetOffsetClockwise();

        var stepDirectionIndex = _stepDirectionIndexes[direction];
        var targetCubeCoords = actorHexNode.CubeCoords + directions[stepDirectionIndex];

        var targetNode = neighborNodes.SingleOrDefault(x => x.CubeCoords == targetCubeCoords);
        return targetNode;
    }
}
