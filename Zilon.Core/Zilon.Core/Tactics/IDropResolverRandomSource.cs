namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Источник рандома при работе для сервиса выбора дропа.
    /// </summary>
    public interface IDropResolverRandomSource
    {
        /// <summary>
        /// Выбрасывает значение, по которому выбирается запись в таблице дропа согласно весу.
        /// </summary>
        /// <param name="totalWeight"> Суммарный вес таблицы дропа (с учётом модификаторов). </param>
        /// <returns> Результат броска. </returns>
        int RollWeight(int totalWeight);
        
        /// <summary>
        /// Случайно выбирает мощь/качетво/уровень экипировки в указанном диапазоне.
        /// </summary>
        /// <param name="minPower"> Минимальное значение мощи. </param>
        /// <param name="maxPower"> Максимальное значение мощи. </param>
        /// <returns> Выбранное значение мощи. </returns>
        int RollEquipmentPower(int minPower, int maxPower);

        /// <summary>
        /// Случайно выбирает количество единиц ресурса в указанном диапазоне.
        /// </summary>
        /// <param name="minPower"> Минимальное количество единиц ресурса. </param>
        /// <param name="maxPower"> Максимальное количество единиц ресурса. </param>
        /// <returns> Выбранное количество единиц ресурса. </returns>
        int RollResourceCount(int minCount, int maxCount);
    }
}
