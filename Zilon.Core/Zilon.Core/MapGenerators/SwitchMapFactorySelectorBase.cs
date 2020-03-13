using Zilon.Core.Schemes;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Базовый селектор фабрики карт, основанный на switch-case.
    /// </summary>
    public abstract class SwitchMapFactorySelectorBase : IMapFactorySelector
    {
        /// <summary>
        /// Возвращает генератор карты.
        /// </summary>
        /// <param name="sectorScheme">Схема сектора, на основе которой будет принято решение,
        /// какой генератор карты использовать.</param>
        /// <returns> Возвращает фабрику карт для сектора. </returns>
        public IMapFactory GetMapFactory(ISectorSubScheme sectorScheme)
        {
            if (sectorScheme is null)
            {
                throw new System.ArgumentNullException(nameof(sectorScheme));
            }

            if (sectorScheme.MapGeneratorOptions == null)
            {
                //TODO Прописать для всех схем конкретный генератор.
                // После явного прописывания здесь нужно будет выбрасывать исключение.
                return RoomMapFactory;
            }

            switch (sectorScheme.MapGeneratorOptions.MapGenerator)
            {
                case SchemeSectorMapGenerator.Room:
                    return RoomMapFactory;

                case SchemeSectorMapGenerator.CellularAutomaton:
                    return CellularAutomatonMapFactory;

                //TODO Прописать для всех схем конкретный генератор.
                // После явного прописывания здесь нужно будет выбрасывать исключение.
                default:
                    return RoomMapFactory;
            }
        }

        /// <summary>
        /// Экземпляр фабрики, генерирующей карты на основе клеточного автомата.
        /// </summary>
        protected abstract IMapFactory CellularAutomatonMapFactory { get; }

        /// <summary>
        /// Экземпляр фабрики, генерирующий карты на основе прямоугольных комнат.
        /// </summary>
        protected abstract IMapFactory RoomMapFactory { get; }
    }
}
