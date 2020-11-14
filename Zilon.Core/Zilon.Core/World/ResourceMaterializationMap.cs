using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Graphs;

namespace Zilon.Core.World
{
    public sealed class ResourceMaterializationMap : IResourceMaterializationMap
    {
        private const float START_RESOURCE_SHARE = 0.10f;
        private readonly IDice _dice;

        private readonly Dictionary<ISectorNode, IResourceDepositData> _map;

        public ResourceMaterializationMap(IDice dice)
        {
            _dice = dice ?? throw new System.ArgumentNullException(nameof(dice));

            _map = new Dictionary<ISectorNode, IResourceDepositData>();
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
                IResourceDepositData data = CreateStartResourceData();

                _map[sectorNode] = data;

                return data;
            }
            else
            {
                IResourceDepositData data = CalcCurrentResouceData(sectorNode);

                _map[sectorNode] = data;

                return data;
            }
        }

        private IResourceDepositData CalcCurrentResouceData(ISectorNode sectorNode)
        {
            var neighborNodes = sectorNode.Biome.GetNext(sectorNode);
            ResourceDepositDataItem[] items = CalcAverageResourceByNeightbors(neighborNodes);
            items = AddNewResourceIfNeed(items);

            ResourceDepositData data = new ResourceDepositData(items);
            return data;
        }

        private ResourceDepositDataItem[] AddNewResourceIfNeed(ResourceDepositDataItem[] items)
        {
            var newRoll = _dice.RollD6();

            if (!items.Any() || newRoll > 3)
            {
                var itemsNew = new List<ResourceDepositDataItem>(items);
                var availableResources = new List<SectorResourceType>
                {
                    SectorResourceType.CherryBrushes,
                    SectorResourceType.Iron,
                    SectorResourceType.Stones,
                    SectorResourceType.WaterPuddles
                };

                foreach (var res in availableResources)
                {
                    var roll = _dice.RollD6();
                    if (roll == 6)
                    {
                        itemsNew.Add(new ResourceDepositDataItem(res, START_RESOURCE_SHARE));
                    }
                }

                items = itemsNew.ToArray();
            }

            return items;
        }

        private ResourceDepositDataItem[] CalcAverageResourceByNeightbors(IEnumerable<IGraphNode> neighborNodes)
        {
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
                        dict[item.ResourceType] = new List<float> {item.Share};
                    }
                }
            }

            var totalResources = new Dictionary<SectorResourceType, float>();
            foreach (var keyValue in dict)
            {
                totalResources[keyValue.Key] = keyValue.Value.Average();
                var diff = _dice.Roll(-25, 25) * 0.01f;
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
                foreach (var keyValue in totalResources.ToArray())
                {
                    totalResources[keyValue.Key] = totalResources[keyValue.Key] / sumShare;
                }
            }

            var items = totalResources.Select(x => new ResourceDepositDataItem(x.Key, x.Value)).ToArray();
            return items;
        }

        private static IResourceDepositData CreateStartResourceData()
        {
            ResourceDepositDataItem[] items = new[]
            {
                new ResourceDepositDataItem(SectorResourceType.Iron, START_RESOURCE_SHARE),
                new ResourceDepositDataItem(SectorResourceType.Stones, START_RESOURCE_SHARE),
                new ResourceDepositDataItem(SectorResourceType.WaterPuddles, START_RESOURCE_SHARE),
                new ResourceDepositDataItem(SectorResourceType.CherryBrushes, START_RESOURCE_SHARE)
            };
            ResourceDepositData data = new ResourceDepositData(items);
            return data;
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