using System.Collections.Generic;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Регион города. Содержит структуры. На содержание каждого региона в городе требуется определённое количество бюджета.
    /// Регион нужно сначала разработать. Время разработчик в итерациях будет зависить от узла, на который регион размещается.
    /// В зависимости от узла в привинции региону будут доступны определнные ресурсы и бонусы.
    /// Например, шахту железной руды можно разместить только в районе, который лежит на руде.
    /// Или регион на болоте будет генерировать антисанитарию.
    /// </summary>
    public sealed class LocalityRegion
    {
        public LocalityRegion()
        {
            Structures = new List<ILocalityStructure>();
        }

        public List<ILocalityStructure> Structures { get; }
    }
}
