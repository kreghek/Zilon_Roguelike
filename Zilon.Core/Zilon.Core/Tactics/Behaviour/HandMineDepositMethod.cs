using System;
using System.Linq;

using Zilon.Core.StaticObjectModules;

namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class HandMineDepositMethod : IMineDepositMethod
    {
        public HandMineDepositMethod()
        {
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

            deposit.Mine();

            return new SuccessMineDepositResult();
        }
    }
}
