using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Core.Tactics.Behaviour
{
    public class IdleTask : ActorTaskBase
    {
        /// <summary>
        /// Минимальное время простоя.
        /// </summary>
        private const int IdleMin = 2;

        /// <summary>
        /// Максимальное время простоя.
        /// </summary>
        private const int IdleMax = 5;

        /// <summary>
        /// Текущий счётчик простоя.
        /// </summary>
        private int _counter;

        public IdleTask(IActor actor, IDecisionSource decisionSource) : base(actor)
        {
            _counter = decisionSource.SelectIdleDuration(IdleMin, IdleMax);
        }

        public override void Execute()
        {
            _counter--;

            if (_counter <= 0)
            {
                IsComplete = true;
            }
        }
    }
}
