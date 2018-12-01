using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tactics
{
    public class SectorManager : ISectorManager
    {
        private readonly ISectorProceduralGenerator _generator;

        public SectorManager(ISectorProceduralGenerator generator)
        {
            _generator = generator;
        }

        public ISector CurrentSector { get; private set; }



        public void CreateSector()
        {
            CurrentSector = _generator.Generate();
        }
    }
}
