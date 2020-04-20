using System.Collections.Generic;
using System.Linq;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Graphs;

namespace Zilon.Core.World
{
    public sealed class ResourceMaterializationMap : IResourceMaterializationMap
    {
        private readonly Dictionary<ISectorNode, IResourceDepositData> _map;
        private readonly IDice _dice;

        public ResourceMaterializationMap(IDice dice)
        {
            _dice = dice ?? throw new System.ArgumentNullException(nameof(dice));
        }

        public IResourceDepositData GetDepositData(ISectorNode sectorNode)
        {
            if (sectorNode is null)
            {
                throw new System.ArgumentNullException(nameof(sectorNode));
            }

            // Алгоритм.
            // Первое, что делаем, это проверяем, не является ли узел стартовым. То есть еще ничего нет.
            // В этом случае задаём случайные стартовые ресурсы.
            // Если не стартовый, тогда собираем данные с соседних, уже сгенерированных узлов.
            // Для каждого ресурса из соседних получаем усреднённое значение.
            // Затем, для каждого ресурса либо снижаем долю, либо увеличиваем.

            if (!_map.Any())
            {
                var items = new[] {
                    new ResourceDepositDataItem(SectorResourceType.Iron, 0.10f),
                    new ResourceDepositDataItem(SectorResourceType.Stones, 0.10f),
                    new ResourceDepositDataItem(SectorResourceType.WaterPuddles, 0.10f),
                    new ResourceDepositDataItem(SectorResourceType.CherryBrushes, 0.10f),
                };
                var data = new ResourceDepositData(items);

                _map[sectorNode] = data;

                return data;
            }
            else
            {
                var neighborNodes = sectorNode.Biome.GetNext(sectorNode);
                var neighborResourceDatas = GetNeighborResourceData(neighborNodes);

                var dict = new Dictionary<SectorResourceType, List<float>>();
                foreach (var data1 in neighborResourceDatas)
                {
                    foreach (var item in data1.Items)
                    {
                        if (dict.TryGetValue(item.ResourceType, out var resourceShareList))
                        {
                            resourceShareList.Add(item.Share);
                        }
                        else
                        {
                            dict[item.ResourceType] = new List<float>() { item.Share };
                        }
                    }
                }

                var totalResources = new Dictionary<SectorResourceType, float>();
                foreach (var keyValue in dict)
                {
                    totalResources[keyValue.Key] = keyValue.Value.Average();
                    var diff = _dice.Roll(-25, 25) / 10f;
                    var totalValue = totalResources[keyValue.Key] + diff;
                    totalResources[keyValue.Key] = totalValue;

                    if (totalValue < 0)
                    {
                        totalResources.Remove(keyValue.Key);
                    }
                    else if (totalValue > 1)
                    {
                        totalResources[keyValue.Key] = 1;
                    }
                }

                var sumShare = totalResources.Values.Sum();
                if (sumShare > 1)
                {
                    foreach (var keyValue in totalResources)
                    {
                        totalResources[keyValue.Key] = totalResources[keyValue.Key] / sumShare;
                    }
                }

                var items = totalResources.Select(x=>new ResourceDepositDataItem(x.Key, x.Value)).ToArray();

                var data = new ResourceDepositData(items);

                _map[sectorNode] = data;

                return data;
            }
        }

        private IEnumerable<IResourceDepositData> GetNeighborResourceData(IEnumerable<IGraphNode> neighborNodes)
        {
            foreach (var node in neighborNodes.Cast<ISectorNode>())
            {
                if (_map.TryGetValue(node, out var data))
                {
                    yield return data;
                }
            }
        }
    }
}
