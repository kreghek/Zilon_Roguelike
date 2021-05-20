using System;
using System.Threading.Tasks;

namespace Zilon.Core.World
{
    public sealed class GlobeService : IGlobeService
    {
        private readonly IBiomeInitializer _biomeInitializer;

        public GlobeService(IBiomeInitializer biomeInitializer)
        {
            _biomeInitializer = biomeInitializer;
        }

        private async Task ExpandGlobeInternalAsync(IGlobe globe, ISectorNode sectorNode)
        {
            await _biomeInitializer.MaterializeLevelAsync(sectorNode).ConfigureAwait(false);

            // Фиксируем новый узел, как известную, материализованную часть мира.
            // Далее этот узел будет обрабатываться при каждом изменении мира.
            globe.AddSectorNode(sectorNode);
        }

        public Task ExpandGlobeAsync(IGlobe globe, ISectorNode sectorNode)
        {
            if (globe is null)
            {
                throw new ArgumentNullException(nameof(globe));
            }

            if (sectorNode is null)
            {
                throw new ArgumentNullException(nameof(sectorNode));
            }

            if (sectorNode.State == SectorNodeState.SectorMaterialized)
            {
                throw new InvalidOperationException("В этом случае такой узел должен уже быть использован.");
            }

            return ExpandGlobeInternalAsync(globe, sectorNode);
        }
    }
}