using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Core.Tactics.Behaviour
{
    public class IdleTask : IActorTask
    {
        /// <summary>
        /// Минимальное время простоя.
        /// </summary>
        private const int IDLE_MIN = 2;

        /// <summary>
        /// Максимальне время простоя.
        /// </summary>
        private const int IDLE_MAX = 5;

        /// <summary>
        /// Текущий счётчик простоя.
        /// </summary>
        private int _counter;
        private IDecisionSource _decisionSource;

        public IdleTask(IActor actor, IDecisionSource decisionSource)
        {
            Actor = actor;
            _decisionSource = decisionSource;
            _counter = _decisionSource.SelectIdleDuration(IDLE_MIN, IDLE_MAX);
        }

        public IActor Actor { get; }

        public bool IsComplete { get; set; }

        public void Execute()
        {
            _counter--;

            if (_counter <= 0)
            {
                IsComplete = true;
            }
        }
    }
}
