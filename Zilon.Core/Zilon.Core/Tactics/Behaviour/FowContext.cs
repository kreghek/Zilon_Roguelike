using Zilon.Core.Common;
using Zilon.Core.Graphs;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public sealed class FowContext : IFowContext
    {
        private readonly ISectorMap _sectorMap;
        private readonly IStaticObjectManager _staticObjectManager;

        public FowContext(ISectorMap sectorMap, IStaticObjectManager staticObjectManager)
        {
            _sectorMap = sectorMap ?? throw new ArgumentNullException(nameof(sectorMap));
            _staticObjectManager = staticObjectManager ?? throw new ArgumentNullException(nameof(staticObjectManager));
        }

        public IEnumerable<IGraphNode> GetNext(IGraphNode node)
        {
            return _sectorMap.GetNext(node);
        }

        public bool IsTargetVisible(IGraphNode baseNode, IGraphNode targetNode)
        {
            if (baseNode is null)
            {
                throw new ArgumentNullException(nameof(baseNode));
            }

            if (targetNode is null)
            {
                throw new ArgumentNullException(nameof(targetNode));
            }

            if (IsBlockedByObstacle(baseNode, targetNode))
            {
                return false;
            }

            return _sectorMap.TargetIsOnLine(baseNode, targetNode);
        }

        /// <summary>
        ///     Метод првоеряет, нет ли на пути просмотра объектов, закрывающих видимость.
        /// </summary>
        private bool IsBlockedByObstacle(IGraphNode baseNode, IGraphNode targetNode)
        {
            CubeCoords baseCubeCoords = ((HexNode)baseNode).CubeCoords;
            CubeCoords targetCubeCoords = ((HexNode)targetNode).CubeCoords;
            CubeCoords[] line = CubeCoordsHelper.CubeDrawLine(baseCubeCoords, targetCubeCoords);

            foreach (CubeCoords linePoint in line)
            {
                var staticObjects = _staticObjectManager.Items.Where(x => (x.Node as HexNode).CubeCoords == linePoint)
                    .ToArray();
                foreach (var staticObject in staticObjects)
                {
                    if (staticObject.IsSightBlock)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}