using System;

using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanActorTaskSource : IActorTaskSource
    {
        private readonly IActor _currentActor;
        private IActorTask _currentTask;

        private HexNode _targetNode;
        private bool _taskIsActual;
        private IAttackTarget _attackTarget;
        private readonly IDecisionSource _decisionSource;

        public HumanActorTaskSource(IActor startActor, IDecisionSource decisionSource)
        {
            _decisionSource = decisionSource;
            _currentActor = startActor;
        }

        public IActorTask[] GetActorTasks(IMap map, IActorManager actorManager)
        {

            if (_taskIsActual && _currentTask?.IsComplete == true)
            {
                _targetNode = null;
            }

            if (_targetNode != null)
            {
                if (_taskIsActual)
                {
                    return new[] { _currentTask };
                }


                _taskIsActual = true;
                var moveTask = new MoveTask(_currentActor, _targetNode, map);
                _currentTask = moveTask;

                return new[] { _currentTask };
            }

            if (_attackTarget == null)
            {
                return null;
            }

            var attackTask = new AttackTask(_currentActor, _attackTarget, _decisionSource);
            _currentTask = attackTask;

            return new[] { _currentTask };
        }


        /// <summary>
        /// Указать намерение двигаться к указанному узлу.
        /// </summary>
        /// <param name="targetNode"> Целевой узел карты. </param>
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