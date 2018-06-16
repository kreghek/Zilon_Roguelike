using System;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanBehaviourSource : IBehaviourSource
    {
        private readonly IActor _currentActor;
        private ICommand _currentCommand;

        private HexNode _targetNode;
        private bool _moveCommandActual = false;

        public HumanBehaviourSource(IActor startActor)
        {
            _currentActor = startActor;
        }

        public ICommand[] GetCommands(IMap map, IActor[] actors)
        {
            if (_currentCommand != null)
            {
                if (_moveCommandActual && _currentCommand.IsComplete)
                {
                    _targetNode = null;
                }
            }

            if (_targetNode != null)
            {
                if (_moveCommandActual)
                {
                    return new[] { _currentCommand };
                }
                else
                {
                    _moveCommandActual = true;
                    var moveCommand = new MoveToPointCommand(_currentActor, _targetNode, map);
                    _currentCommand = moveCommand;

                    return new[] { _currentCommand };
                }
            }

            return null;
        }

        public void AssignMoveToPointCommand(HexNode targetNode)
        {
            if (targetNode == null)
            {
                throw new ArgumentException(nameof(targetNode));
            }

            if (targetNode != _targetNode)
            {
                _moveCommandActual = false;
                _targetNode = targetNode;
            }
        }
    }
}