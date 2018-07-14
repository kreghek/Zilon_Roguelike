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
        private IPropContainer _propContainer;
        private IOpenContainerMethod _method;
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
                _attackTarget = null;
                _method = null;
                _propContainer = null;
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

            if (_attackTarget != null)
            {
                var attackTask = new AttackTask(_currentActor, _attackTarget, _decisionSource);
                _currentTask = attackTask;
                return new[] { _currentTask };
            }

            if (_propContainer != null)
            {
                var openContainerTask = new OpenContainerTask(_currentActor, _propContainer, _method);
                _currentTask = openContainerTask;
                return new[] { _currentTask };
            }

            return new IActorTask[0];
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

        //TODO Должна быть возможность указывать действие, которым можно атаковать.
        /// <summary>
        /// Указать намерение атаковать цель.
        /// </summary>
        /// <param name="target"> Целевой объект. </param>
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

        public void IntentOpenContainer(IPropContainer container, IOpenContainerMethod method)
        {
            _taskIsActual = false;
            _targetNode = null;
            _attackTarget = null;
            _method = method;
            _propContainer = container;
        }
    }
}