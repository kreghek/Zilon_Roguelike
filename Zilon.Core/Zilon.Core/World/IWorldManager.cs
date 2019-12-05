namespace Zilon.Core.World
{
    /// <summary>
    /// Сервис, управляющий глобальной картой.
    /// </summary>
    public interface IWorldManager
    {
        /// <summary>
        /// Глобальная карта.
        /// </summary>
        Globe Globe { get; set; }

        //TODO Все эти свойства выглядят ненужными
        ///// <summary>
        ///// Текущие сгенерированые провинции относительно ячеек глобальной карты.
        ///// </summary>
        //Dictionary<TerrainCell, GlobeRegion> Regions { get; }

        ///// <summary>
        ///// История генерации мира.
        ///// </summary>
        //GlobeGenerationHistory GlobeGenerationHistory { get; set; }

        ///// <summary>
        ///// Обновление состояния узлов провинции.
        ///// </summary>
        ///// <param name="region">Провинция, которая обновляется.</param>
        //void UpdateRegionNodes(GlobeRegion region);
    }
}
