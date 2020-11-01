using Zilon.Core.Schemes;
using Zilon.Core.Scoring;

namespace Zilon.Core.Persons
{
    /// <summary>
    ///     Персонаж, находящийся под управлением игрока.
    /// </summary>
    public class HumanPerson : PersonBase
    {
        public HumanPerson([NotNull] IPersonScheme scheme, IFraction fraction) : base(fraction)
        {
            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));

            Name = scheme.Sid;
        }

        /// <inheritdoc />
        public override int Id { get; set; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IPersonScheme Scheme { get; }

        public IPlayerEventLogService PlayerEventLogService { get; set; }

        public override PhysicalSize PhysicalSize => PhysicalSize.Size1;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}