using System;

using Zilon.Core.Props;
using Zilon.Core.StaticObjectModules;

namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class ToolMineDepositMethod : IMineDepositMethod
    {
        private readonly Equipment _tool;

        public ToolMineDepositMethod(Equipment tool)
        {
            _tool = tool ?? throw new ArgumentNullException(nameof(tool));
        }

        public IMineDepositResult TryMine(IPropDepositModule deposit)
        {
            if (deposit is null)
            {
                throw new ArgumentNullException(nameof(deposit));
            }

            var requiredToolScheme = deposit.Tool;

            if (requiredToolScheme != _tool.Scheme)
            {
                throw new InvalidOperationException("Попытка выполнить добычу ресурса не подходящим инструментом.");
            }

            deposit.Mine();

            return new SuccessMineDepositResult();
        }
    }
}
