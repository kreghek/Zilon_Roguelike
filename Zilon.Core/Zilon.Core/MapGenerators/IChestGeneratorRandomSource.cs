using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Источник рандома для генератора сундуков в секторе.
    /// </summary>
    public interface IChestGeneratorRandomSource
    {
        /// <summary>
        /// Получение количества сундуков в регионе.
        /// </summary>
        /// <param name="maxCount"> Максимальное количество. </param>
        /// <returns> Возвращает случайное количество. </returns>
        int RollChestCount(int maxCount);

        /// <summary>
        /// Получение случайного узла для размещения сундука в регионе.
        /// </summary>
        /// <param name="nodeCount"> Колчество узлов, из которых выбирать. </param>
        /// <returns> Возвращает индекс случайного узла для размещения сундука. </returns>
        int RollNodeIndex(int nodeCount);

        /// <summary>
        /// Выбирает назначение контейнера.
        /// </summary>
        /// <returns>Возвращает значение перечисления, указывающее текущую редкость контейнера.</returns>
        PropContainerPurpose RollPurpose();
    }
}