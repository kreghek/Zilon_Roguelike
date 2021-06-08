using System;

namespace Zilon.Core.Components
{
    [Flags]
    public enum EquipmentSlotTypes
    {
        /// <summary>
        /// Means slot has no type.
        /// This is looks like error.
        /// </summary>
        None = 0,

        Head = 1,

        Body = 2,

        Hand = 4,

        Aux = 8
    }
}