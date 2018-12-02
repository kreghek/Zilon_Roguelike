using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public static class MapHelper
    {
        //TODO Добавить тест
        //TODO Перенести в карту, чтобы опираться на таблицу проходимости, как в генераторе сектора.
        public static bool CheckNodeAvailability(IMap map, IMapNode currentNode, IMapNode targetNode)
        {
            var targetHexNode = (HexNode)targetNode;
            var currentHexNode = (HexNode)currentNode;

            var line = CubeCoordsHelper.CubeDrawLine(currentHexNode.CubeCoords, targetHexNode.CubeCoords);

            for (var i = 1; i < line.Length; i++)
            {
                var prevPoint = line[i - 1];
                var testPoint = line[i];

                var prevNode = map.Nodes
                    .SingleOrDefault(x => ((HexNode)x).CubeCoords == prevPoint);

                var testNode = map.Nodes
                    .SingleOrDefault(x => ((HexNode)x).CubeCoords == testPoint);

                if (!map.GetNext(prevNode).Contains(testNode))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
