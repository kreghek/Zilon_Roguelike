using System;
using System.Linq;

using Zilon.Core.Props;
using Zilon.Core.StaticObjectModules;

namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class ToolMineDepositMethod : IMineDepositMethod
    {
        private readonly Equipment _tool;
        private readonly IMineDepositMethodRandomSource _mineDepositMethodRandomSource;

        public ToolMineDepositMethod(Equipment tool, IMineDepositMethodRandomSource mineDepositMethodRandomSource)
        {
            _tool = tool ?? throw new ArgumentNullException(nameof(tool));
            _mineDepositMethodRandomSource = mineDepositMethodRandomSource;
        }

        public IMineDepositResult TryMine(IPropDepositModule deposit)
        {
            if (deposit is null)
            {
                throw new ArgumentNullException(nameof(deposit));
            }

            var requiredToolTags = deposit.GetToolTags();

            if (!requiredToolTags.Except(_tool.Scheme.Tags).Any())
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
