using System;
using System.Threading.Tasks;

using Zilon.Core.Tactics;

namespace Zilon.GlobeObserver
{
    public class GenerationSectorManager : ISectorManager
    {
        public ISector CurrentSector { get; set; }

        public Task CreateSectorAsync()
        {
            throw new NotImplementedException();
        }
    }
}
