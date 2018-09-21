using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Tactics;

namespace Zilon.Core.Client
{
    public class SectorManager : ISectorManager
    {
        [ExcludeFromCodeCoverage]
        public ISector CurrentSector { get; set; }
    }
}
