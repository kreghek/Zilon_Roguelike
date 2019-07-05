namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Реализация источника случайного выбора для генератора мирных жителей.
    /// </summary>
    public interface ICitizenGeneratorRandomSource
    {
        /// <summary>
        /// Получение случайного узла для размещения мирного жителя в регионе.
        /// </summary>
        /// <param name="nodeCount"> Количество узлов, из которых выбирать. </param>
        /// <returns> Возвращает индекс случайного узла для размещения мирного жителя. </returns>
        int RollNodeIndex(int nodeCount);

        CitizenType RollCitizenType();
    }

    public enum CitizenType
    {
        Undefined,
        Unintresting,
        Trader,
        QuestGiver
    }
}