using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.World;

namespace Zilon.Core.Tactics.Behaviour
{
    public class IdleTask : ActorTaskBase
    {
        /// <summary>
        /// Минимальное время простоя.
        /// </summary>
        private const int IDLE_MIN = 2;

        /// <summary>
        /// Максимальное время простоя.
        /// </summary>
        private const int IDLE_MAX = 5;

        /// <summary>
        /// Текущий счётчик простоя.
        /// </summary>
        private int _counter;

        public IdleTask(IActor actor, IActorTaskContext context, IDecisionSource decisionSource) : base(actor, context)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            if (decisionSource is null)
            {
                throw new System.ArgumentNullException(nameof(decisionSource));
            }

            _counter = decisionSource.SelectIdleDuration(IDLE_MIN, IDLE_MAX);
        }

        public IdleTask(IActor actor, IActorTaskContext context, int duration) : base(actor, context)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            _counter = duration;
        }

        public override int Cost => GlobeMetrics.IdleDuration;

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