using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public interface IGlobeInitializer
    {
        System.Threading.Tasks.Task<IGlobe> CreateGlobeAsync();
    }

    public interface IGlobe
    {
        void Update();
    }

    public sealed class Globe : IGlobe
    {
        private readonly IList<ISectorNode> _sectorNodes;

        public IEnumerable<ISectorNode> SectorNodes { get; }

        public void AddSectorNode(ISectorNode sectorNode)
        {
            _sectorNodes.Add(sectorNode);
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class GlobeInitializer : IGlobeInitializer
    {
        private readonly IBiomeInitializer _biomeInitializer;
        private readonly ISchemeService _schemeService;

        public GlobeInitializer(IBiomeInitializer biomeInitializer, ISchemeService schemeService)
        {
            _biomeInitializer = biomeInitializer;
            _schemeService = schemeService;
        }

        public async System.Threading.Tasks.Task<IGlobe> CreateGlobeAsync()
        {
            var globe = new Globe();

            var startLocation = _schemeService.GetScheme<ILocationScheme>("init");
            var startBiom = await _biomeInitializer.InitBiomeAsync(startLocation).ConfigureAwait(false);
            var startNode = startBiom.Sectors.First();

            globe.AddSectorNode(startNode);

            return globe;
        }
    }
}
