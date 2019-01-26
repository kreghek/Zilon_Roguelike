using Zilon.Core.WorldGeneration;

namespace Assets.Zilon.Scripts.Services
{
    public class GlobeManager : IGlobeManager
    {
        private readonly IWorldGenerator _generator;

        public GlobeManager(IWorldGenerator generator)
        {
            _generator = generator;
        }

        public Globe CurrentGlobe { get; private set; }

        public void Generate()
        {
            CurrentGlobe = _generator.Generate();
        }
    }
}
