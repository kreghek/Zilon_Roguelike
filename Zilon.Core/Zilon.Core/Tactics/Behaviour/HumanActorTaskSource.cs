using System;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanActorTaskSource : IActorTaskSource
    {
        private readonly IActor _currentActor;
        private IActorTask _currentCommand;

        private HexNode _targetNode;
        private bool _moveCommandActual = false;

        public HumanActorTaskSource(IActor startActor)
        {
            _currentActor = startActor;
        }

        public IActorTask[] GetIntents(IMap map, IActor[] actors)
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
                    var moveCommand = new MoveTask(_currentActor, _targetNode, map);
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