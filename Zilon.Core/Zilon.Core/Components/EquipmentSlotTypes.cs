using System;

using JetBrains.Annotations;

namespace Zilon.Core.Components
{
    [PublicAPI]
    [Flags]
    public enum EquipmentSlotTypes
    {
        Head = 1,
        Body = 2,
        Hand = 4,
        Aux = 8
    }
}
