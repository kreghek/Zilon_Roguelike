﻿using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonModules
{
    public class EquipmentModule : EquipmentModuleBase
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        // This value was assigned in base class.
        public EquipmentModule([NotNull][ItemNotNull] IReadOnlyCollection<PersonSlotSubScheme> slots) : base(slots)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public override PersonSlotSubScheme[] Slots { get; protected set; }

        protected override void ValidateSetEquipment(Equipment equipment, int slotIndex)
        {
            var slot = Slots[slotIndex];

            if (!EquipmentCarrierHelper.CheckSlotCompability(equipment, slot))
            {
                throw new ArgumentException(
                    $"Для экипировки указан слот {slot}, не подходящий для данного типа предмета {equipment}.");
            }

            if (!EquipmentCarrierHelper.CheckDualCompability(this, equipment, slotIndex))
            {
                throw new InvalidOperationException(
                    $"Попытка экипировать предмет {equipment}, несовместимый с текущий экипировкой.");
            }

            if (!EquipmentCarrierHelper.CheckShieldCompability(this, equipment, slotIndex))
            {
                throw new InvalidOperationException("Попытка экипировать два щита.");
            }
        }
    }
}