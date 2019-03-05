using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zilon.Core.Props
{
    public class EquipmentDurableService : IEquipmentDurableService
    {
        private const int SUCCESS_TURN_RESIST = 2;
        private const int SUCCESS_USE_RESIST = 6;
        private const float FIELD_REDUCE_MAX = 0.1f;
        private const float MASTER_REDUCE_MAX = 0.1f;

        private readonly IEquipmentDurableServiceRandomSource _randomSource;

        public EquipmentDurableService(IEquipmentDurableServiceRandomSource randomSource)
        {
            _randomSource = randomSource;
        }

        public void Repair(IProp repairResource, Equipment equipment)
        {
            var maxDurable = equipment.Durable.Range.Max;
            var reduceValue = (int)Math.Round(maxDurable * FIELD_REDUCE_MAX) + 1;
            var newMaxDurable = maxDurable - reduceValue;

            if (newMaxDurable <= 0)
            {
            }

            equipment.Durable.ChangeStatRange(0, maxDurable - reduceValue);
        }

        public void UpdateByTurn(Equipment equipment)
        {
            var resistRoll = _randomSource.RollTurnResist(equipment);
            if (resistRoll < SUCCESS_TURN_RESIST)
            {
                equipment.Durable.Value--;
            }
        }

        public void UpdateByUse(Equipment equipment)
        {
            var resistRoll = _randomSource.RollUseResist(equipment);
            if (resistRoll < SUCCESS_USE_RESIST)
            {
                equipment.Durable.Value--;
            }
        }

        public class RepairResult {
            bool Destroyed;
        }
    }
}
