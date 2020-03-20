namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class SectorSnapshot
    {
        public SectorSnapshot(ISector sector)
        {
            Sector = sector;
        }

        public ISector Sector { get; }
    }
}
