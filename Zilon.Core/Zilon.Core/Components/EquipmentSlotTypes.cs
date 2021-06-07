using System;

namespace Zilon.Core.Components
{
    [Flags]
    public enum EquipmentSlotTypes
    {
        Undefined = 0,
        Head = 1,
        Body = 2,
        Hand = 4,
        Aux = 8
    }
}