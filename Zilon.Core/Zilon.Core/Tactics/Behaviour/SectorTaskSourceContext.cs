namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class SectorTaskSourceContext : ISectorTaskSourceContext
    {
        public SectorTaskSourceContext(ISector sector)
        {
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
        }

        public ISector Sector { get; }
    }
}