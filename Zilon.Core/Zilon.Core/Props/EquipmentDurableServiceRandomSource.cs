using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Props
{
    public sealed class EquipmentDurableServiceRandomSource : IEquipmentDurableServiceRandomSource
    {
        private readonly IDice _dice;

        public EquipmentDurableServiceRandomSource(IDice dice)
        {
            _dice = dice;
        }

        public int RollTurnResist(Equipment equipment)
        {
            return _dice.Roll(6);
        }

        public int RollUseResist(Equipment equipment)
        {
            return _dice.Roll(6);
        }
    }
}