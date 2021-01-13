using System;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.StaticObjectModules;

namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class MineDepositMethodRandomSource : IMineDepositMethodRandomSource
    {
        private readonly IDice _dice;

        public MineDepositMethodRandomSource(IDice dice)
        {
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));
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

        public bool RollSuccess(DepositMiningDifficulty difficulty)
        {
            var successValue = GetSuccessValue(difficulty);
            var rollValue = _dice.Roll2D6();
            return rollValue >= successValue;
        }
    }
}