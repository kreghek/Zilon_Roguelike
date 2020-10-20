using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Core.Tactics.Behaviour
{
    public class IdleTask : ActorTaskBase
    {
        /// <summary>
        /// Минимальное время простоя.
        /// </summary>
        private const int _idleMin = 2;

        /// <summary>
        /// Максимальное время простоя.
        /// </summary>
        private const int _idleMax = 5;

        /// <summary>
        /// Текущий счётчик простоя.
        /// </summary>
        private int _counter;

        public IdleTask(IActor actor, IDecisionSource decisionSource) : base(actor)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            if (decisionSource is null)
            {
                throw new System.ArgumentNullException(nameof(decisionSource));
            }

            _counter = decisionSource.SelectIdleDuration(_idleMin, _idleMax);
        }

        public IdleTask(IActor actor, int duration) : base(actor)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            _counter = duration;
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