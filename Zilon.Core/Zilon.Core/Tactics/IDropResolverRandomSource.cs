namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Источник рандома при работе для сервиса выбора дропа.
    /// </summary>
    public interface IDropResolverRandomSource
    {
        /// <summary>
        /// Случайно выбирает количество единиц ресурса в указанном диапазоне.
        /// </summary>
        /// <param name="minCount"> Минимальное количество единиц ресурса. </param>
        /// <param name="maxCount"> Максимальное количество единиц ресурса. </param>
        /// <returns> Выбранное количество единиц ресурса. </returns>
        int RollResourceCount(int minCount, int maxCount);

        /// <summary>
        /// Выбрасывает значение, по которому выбирается запись в таблице дропа согласно весу.
        /// </summary>
        /// <param name="totalWeight"> Суммарный вес таблицы дропа (с учётом модификаторов). </param>
        /// <returns> Результат броска. </returns>
        int RollWeight(int totalWeight);
    }
}