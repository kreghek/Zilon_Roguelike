using System;

using Zilon.Core.Localization;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Персонаж, находящийся под управлением игрока.
    /// </summary>
    public class HumanPerson : PersonBase
    {
        public HumanPerson(IPersonScheme scheme, IFraction fraction) : base(fraction)
        {
            Scheme = scheme;

            Name = scheme.Sid ?? throw new InvalidOperationException();
        }

        /// <inheritdoc />
        public override int Id { get; set; }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Temporary property to show template name.
        /// </summary>
        public ILocalizedString? PersonEquipmentTemplate { get; set; }

        /// <summary>
        /// Temporary property to show template name.
        /// </summary>
        public ILocalizedString? PersonEquipmentDescriptionTemplate { get; set; }

        public override PhysicalSizePattern PhysicalSize => PhysicalSizePattern.Size1;

        public IPlayerEventLogService? PlayerEventLogService { get; set; }

        /// <inheritdoc />
        public IPersonScheme Scheme { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}