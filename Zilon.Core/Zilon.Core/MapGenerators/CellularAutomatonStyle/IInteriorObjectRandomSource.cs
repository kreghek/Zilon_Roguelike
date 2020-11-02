namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Базовая реализация источника рандома при работе с элементами интерьера.
    /// </summary>
    public interface IInteriorObjectRandomSource
    {
        /// <summary>
        /// Случайный выбор координат для размещения элемента интерьера.
        /// </summary>
        /// <param name="regionDraftCoords"> Координаты региона, среди которых можно выбирать позиции элементов интерьера. </param>
        /// <returns> Возвращает набор метаданных об элементах интерьера. </returns>
        InteriorObjectMeta[] RollInteriorObjects(OffsetCoords[] regionDraftCoords);
    }
}