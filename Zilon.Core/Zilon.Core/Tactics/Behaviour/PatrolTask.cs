using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Задача на патрулирование.
    /// </summary>
    public class PatrolTask : IActorTask
    {
        private IMapNode _targetNode;
        private MoveTask _moveTask;
        private readonly IMap _map;
        private readonly IDecisionSource _decisionSource;
        private readonly IMapNode[] _patrolNodes;

        public IActor Actor { get; }

        public bool IsComplete { get; set; }

        public PatrolTask(IActor actor,
            IMap map,
            IDecisionSource decisionSource,
            IMapNode[] patrolNodes)
        {
            Actor = actor;
            _map = map;
            _decisionSource = decisionSource;
            _patrolNodes = patrolNodes;
            SelectPatrolPoint();
        }

        private void SelectPatrolPoint()
        {
            _targetNode = _decisionSource.SelectPatrolPoint(_map);
        }

        public void Execute()
        {
            if (_moveTask == null)
            {
                _moveTask = new MoveTask(Actor, _targetNode, _map);
            }

            IsComplete = _moveTask.IsComplete;
        }
    }
}
