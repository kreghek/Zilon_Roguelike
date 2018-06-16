using Zilon.Core.Tactics.Map;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanBehaviourSource : IBehaviourSource
    {
        private IActor _currentActor;
        private ICommand _currentCommand;

        private MapNode _targetNode;
        private bool _moveCommandActual = false;

        public HumanBehaviourSource(IActor startActor)
        {
            _currentActor = startActor;
        }

        public ICommand[] GetCommands(CombatMap map, IActor[] actors)
        {
            if (_currentCommand != null)
            {
                if (_currentCommand.IsComplete)
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

        public void AssignMoveToPointCommand(MapNode targetNode)
        {
            if (targetNode != _targetNode)
            {
                _moveCommandActual = false;
                _targetNode = targetNode;
            }
        }
    }
}