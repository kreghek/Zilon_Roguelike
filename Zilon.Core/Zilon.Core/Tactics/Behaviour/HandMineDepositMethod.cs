using System;
using System.Linq;

using Zilon.Core.StaticObjectModules;

namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class HandMineDepositMethod : IMineDepositMethod
    {
        private readonly IMineDepositMethodRandomSource _mineDepositMethodRandomSource;

        public HandMineDepositMethod(IMineDepositMethodRandomSource mineDepositMethodRandomSource)
        {
            _mineDepositMethodRandomSource = mineDepositMethodRandomSource;
        }

        public IMineDepositResult TryMine(IPropDepositModule deposit)
        {
            if (deposit is null)
            {
                throw new ArgumentNullException(nameof(deposit));
            }

            if (deposit.GetToolTags().Any())
            {
                throw new InvalidOperationException("Попытка выполнить добычу ресурса не подходящим инструментом.");
            }

            var isSuccessfulMining = _mineDepositMethodRandomSource.RollSuccess(deposit.Difficulty);
            if (isSuccessfulMining)
            {
                deposit.Mine();

                return new SuccessMineDepositResult();
            }
            else
            {
                return new FailureMineDepositResult();
            }
        }
    }
}
