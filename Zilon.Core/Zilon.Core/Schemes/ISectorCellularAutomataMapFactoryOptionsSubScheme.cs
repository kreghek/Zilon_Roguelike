namespace Zilon.Core.Schemes
{
    /// <summary>
    ///     Схема параметров генерации карты на основе клеточного автомата.
    /// </summary>
    public interface ISectorCellularAutomataMapFactoryOptionsSubScheme : ISectorMapFactoryOptionsSubScheme
    {
        /// <summary>
        ///     Ширина матрицы, на которой будет работать клеточный автомат.
        /// </summary>
        int MapWidth { get; }

        /// <summary>
        ///     Высота матрицы, на которой будет работать клеточный автомат.
        /// </summary>
        int MapHeight { get; }

        /// <summary>
        ///     Шанс, что на первоначальной карте клетка будет живой.
        ///     Указывается в диапазоне 0..100.
        /// </summary>
        int ChanceToStartAlive { get; }
    }
}