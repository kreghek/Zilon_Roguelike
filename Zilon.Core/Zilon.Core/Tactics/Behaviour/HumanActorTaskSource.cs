using System;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanActorTaskSource : IActorTaskSource
    {
        private readonly IActor _currentActor;
        private IActorTask _currentTask;

        private HexNode _targetNode;
        private bool _taskIsActual = false;
        private IAttackTarget _attackTarget;

        public HumanActorTaskSource(IActor startActor)
        {
            _currentActor = startActor;
        }

        public IActorTask[] GetActorTasks(IMap map, IActor[] actors)
        {
            if (_currentTask != null)
            {
                if (_taskIsActual && _currentTask.IsComplete)
                {
                    _targetNode = null;
                }
            }

            if (_targetNode != null)
            {
                if (_taskIsActual)
                {
                    return new[] { _currentTask };
                }
                else
                {
                    _taskIsActual = true;
                    var moveTask = new MoveTask(_currentActor, _targetNode, map);
                    _currentTask = moveTask;

                    return new[] { _currentTask };
                }
            }

            if (_attackTarget != null)
            {
                var attackTask = new AttackTask(_currentActor, _attackTarget);
                _currentTask = attackTask;

                return new[] { _currentTask };
            }

            return null;
        }

        public void IntentMove(HexNode targetNode)
        {
            if (targetNode == null)
            {
                throw new ArgumentException(nameof(targetNode));
            }

            _attackTarget = null;

            if (targetNode != _targetNode)
            {
                _taskIsActual = false;
                _targetNode = targetNode;
            }
        }

        public void IntentAttack(IAttackTarget target)
        {
            //Отключаю это предупреждение, иначе получается кривой код.
#pragma warning disable IDE0016 // Use 'throw' expression
            if (target == null)
            {
                throw new ArgumentException(nameof(target));
            }
#pragma warning restore IDE0016 // Use 'throw' expression

            _taskIsActual = false;
            _targetNode = null;
            _attackTarget = target;
        }
    }
}