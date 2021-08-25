﻿namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
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
        /// <param name="checkPass">
        /// Checks passability of the sector after static object placement. This is optimization for the
        /// open sectors.
        /// </param>
        /// <returns> Возвращает набор метаданных об элементах интерьера. </returns>
        InteriorObjectMeta[] RollInteriorObjects(OffsetCoords[] regionDraftCoords, bool checkPass);
    }
}