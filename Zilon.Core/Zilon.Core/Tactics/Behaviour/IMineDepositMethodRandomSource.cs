using Zilon.Core.CommonServices.Dices;
using Zilon.Core.StaticObjectModules;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    ///     Источник случайностей для методов добычи.
    /// </summary>
    public interface IMineDepositMethodRandomSource
    {
        /// <summary>
        ///     Случайно выбирает усепшность добычи.
        /// </summary>
        /// <returns>true, если добыча прошла успешно. false - в ином случае.</returns>
        bool RollSuccess(DepositMiningDifficulty difficulty);
    }

    public sealed class MineDepositMethodRandomSource : IMineDepositMethodRandomSource
    {
        private readonly IDice _dice;

        public MineDepositMethodRandomSource(IDice dice)
        {
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));
        }

        public bool RollSuccess(DepositMiningDifficulty difficulty)
        {
            var successValue = GetSuccessValue(difficulty);
            var rollValue = _dice.Roll2D6();
            return rollValue >= successValue;
        }

        private static int GetSuccessValue(DepositMiningDifficulty difficulty)
        {
            switch (difficulty)
            {
                case DepositMiningDifficulty.Easy:
                    return 3;

                case DepositMiningDifficulty.Moderately:
                    return 6;

                case DepositMiningDifficulty.Hard:
                    return 9;

                case DepositMiningDifficulty.Impossible:
                    return 13;

                case DepositMiningDifficulty.Undefined:
                default:
                    throw new InvalidOperationException($"Неизвестная сложность добычи {difficulty}");
            }
        }
    }
}