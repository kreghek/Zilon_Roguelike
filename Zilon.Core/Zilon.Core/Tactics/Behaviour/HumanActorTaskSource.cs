using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanActorTaskSource : IActorTaskSource
    {
        private readonly IActor _currentActor;
        private IActorTask _currentTask;

        //TODO обобщить намерения.
        private HexNode _targetNode;
        private bool _taskIsActual;
        private IAttackTarget _attackTarget;
        private IPropContainer _propContainer;
        private IOpenContainerMethod _method;
        private PropTransfer[] _transfers;
        private Equipment _equipment;
        private int _slotIndex;

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

            if (_propContainer != null && _method != null)
            {
                var openContainerTask = new OpenContainerTask(_currentActor, _propContainer, _method);
                _currentTask = openContainerTask;
                return new[] { _currentTask };
            }

            if (_transfers != null)
            {
                var inventory = _currentActor.Inventory;

                if (inventory == null)
                {
                    throw new InvalidOperationException($"Для данного актёра {_currentActor} не задан инвентарь.");
                }

                _currentTask = new TransferPropsTask(_currentActor, _transfers);
                return new[] { _currentTask };
            }

            if (_equipment != null)
            {
                _currentTask = new EquipTask(_currentActor, _equipment, _slotIndex);
                return new[] { _currentTask };
            }

            return new IActorTask[0];
        }


        /// <summary>
        /// Указать намерение двигаться к указанному узлу.
        /// </summary>
        /// <param name="targetNode"> Целевой узел карты. </param>
        public virtual void IntentMove(HexNode targetNode)
        {
            if (targetNode == null)
            {
                throw new ArgumentException(nameof(targetNode));
            }

            _attackTarget = null;

            if (targetNode != _targetNode)
            {
                _taskIsActual = false;
                ClearCurrentTask();

                _targetNode = targetNode;
            }
        }

        //TODO Должна быть возможность указывать действие, которым можно атаковать.
        /// <summary>
        /// Указать намерение атаковать цель.
        /// </summary>
        /// <param name="target"> Целевой объект. </param>
        public virtual void IntentAttack(IAttackTarget target)
        {
            //Отключаю это предупреждение, иначе получается кривой код.
#pragma warning disable IDE0016 // Use 'throw' expression
            if (target == null)
            {
                throw new ArgumentException(nameof(target));
            }
#pragma warning restore IDE0016 // Use 'throw' expression

            ClearCurrentTask();

            _attackTarget = target;
        }

        /// <summary>
        /// Намерение открыть контейнер в секторе.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="method"></param>
        public virtual void IntentOpenContainer(IPropContainer container, IOpenContainerMethod method)
        {
            ClearCurrentTask();
            _method = method;
            _propContainer = container;
        }

        /// <summary>
        /// Hамерение перенести предметы между хранилищами (инвентарь-сундук-пол).
        /// </summary>
        /// <param name="props"></param>
        public virtual void IntentTransferProps(IEnumerable<PropTransfer> transfers)
        {
            //TODO Сделать генерацию контейнеров для сброшенных на пол пердметов.
            ClearCurrentTask();
            _transfers = transfers.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equipment"></param>
        /// <param name="slotIndex"></param>
        public virtual void IntentEquip(Equipment equipment, int slotIndex)
        {
            ClearCurrentTask();
            _equipment = equipment;
            _slotIndex = slotIndex;
        }

        private void ClearCurrentTask()
        {
            _taskIsActual = false;
            _targetNode = null;
            _attackTarget = null;
            _method = null;
            _propContainer = null;
        }
    }
}