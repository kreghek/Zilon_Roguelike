﻿using System.Collections.Generic;

using Zilon.Core.Schemes;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Источник рандома для генератора монстров в секторе.
    /// </summary>
    public interface IMonsterGeneratorRandomSource
    {
        /// <summary> Выбирает случайную редкость моснтра. </summary>
        /// <returns> 2 - чемпион, 1 - редкий, 0 - обычный. </returns>
        int RollRarity();

        /// <summary> Выбирает случайную схему монстра среди доступных. </summary>
        /// <param name="availableMonsterSchemes"> Доступные схемы монстров.
        /// Расчитываются исходя из схемы сектора и выбранной редкости. </param>
        /// <returns> Возвращает схему монстра. </returns>
        IMonsterScheme RollMonsterScheme(IEnumerable<IMonsterScheme> availableMonsterSchemes);

        /// <summary> Выбирает случайное количество монстров в секторе. </summary>
        /// <param name="regionMaxCount"> Максимальное количество монстров в секторе. </param>
        /// <returns> Возвращает слуяайное количество монстров в секторе от 0 до указанного максимального числа. </returns>
        int RollRegionCount(int regionMaxCount);

        /// <summary> Выбирает слуяайный индекс узла, в который будет размещён монстр. </summary>
        /// <param name="count"> Количество узлов в коллекции доступных узлов. </param>
        /// <returns> Индекс узла карты. </returns>
        int RollNodeIndex(int count);
    }
}
