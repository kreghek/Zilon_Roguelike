namespace Zilon.Core.World
{
    public interface IResourceMaterializationMap
    {
        /// <summary>
        /// Предоставляет информацию о ресурсах, которые нужно сгенерировать при материализации сектора.
        /// </summary>
        /// <param name="sectorNode"> Узел графа биома. </param>
        /// <returns>Возрашает данные о текущем соотношении ресурсов в секторе.</returns>
        /// <remarks>
        /// Идея в том, чтобы было менее хаотичное разпределение ресурсов по секторам.
        /// 1. При переходе между секторами количество какого-то одного ресурса должно увеличиваться, а других - падать.
        /// 2. Ресурс, которые давное не появлялся, должен с большей вероятностью начать генерироваться. И наоборот.
        /// Если какой-то ресурс только что достиг пика плотности, то необходимо снизить вероятность его появления в следующих секторах.
        /// Иначе можут быть профицит одного ресурса над другими.
        /// Вот в чём назначение данного объекта - распределять информацию о ресурсах наиболее привлекательным образом.
        /// </remarks>
        IResourceDepositData GetDepositData(ISectorNode sectorNode);
    }

    public sealed class ResourceMaterializationMap : IResourceMaterializationMap
    {
        public IResourceDepositData GetDepositData(ISectorNode sectorNode)
        {
            var items = new[] { 
                new ResourceDepositDataItem(SectorResourceType.Iron, 10),
                new ResourceDepositDataItem(SectorResourceType.Stones, 10),
                new ResourceDepositDataItem(SectorResourceType.WaterPuddles, 10),
                new ResourceDepositDataItem(SectorResourceType.CherryBrushes, 10),
            };
            return new ResourceDepositData(items);
        }
    }
}
