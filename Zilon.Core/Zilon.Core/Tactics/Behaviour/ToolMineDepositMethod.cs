using System;

using Zilon.Core.Common;
using Zilon.Core.Props;
using Zilon.Core.StaticObjectModules;

namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class ToolMineDepositMethod : IMineDepositMethod
    {
        private readonly IMineDepositMethodRandomSource _mineDepositMethodRandomSource;
        private readonly Equipment _tool;

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
            var hasAllTags = EquipmentHelper.HasAllTags(_tool.Scheme.Tags, requiredToolTags);
            if (!hasAllTags)
            {
                throw new InvalidOperationException("Попытка выполнить добычу ресурса не подходящим инструментом.");
            }

            var isSuccessfulMining = _mineDepositMethodRandomSource.RollSuccess(deposit.Difficulty);
            if (isSuccessfulMining)
            {
                deposit.Mine();

                return new SuccessMineDepositResult();
            }

            return new FailureMineDepositResult();
        }
    }
}