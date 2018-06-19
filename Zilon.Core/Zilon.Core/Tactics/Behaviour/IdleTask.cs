using Zilon.Core.Services.Dice;
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

        public IdleTask(IDice dice)
        {
            _counter = dice.Roll(IDLE_MAX) + IDLE_MIN - 1;
        }

        public IdleTask(IDecisionSource decisionSource)
        {
            _decisionSource = decisionSource;
        }

        public IActor Actor { get; }

        public bool IsComplete { get; set; }

        public void Execute()
        {
            if (_counter > 0)
            {
                _counter--;
            }
            else
            {
                IsComplete = true;
            }
        }
    }
}
