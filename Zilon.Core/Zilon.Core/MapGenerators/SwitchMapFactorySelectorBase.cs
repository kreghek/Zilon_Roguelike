using System;

using Zilon.Core.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Базовый селектор фабрики карт, основанный на switch-case.
    /// </summary>
    public abstract class SwitchMapFactorySelectorBase : IMapFactorySelector
    {
        /// <summary>
        /// Экземпляр фабрики, генерирующей карты на основе клеточного автомата.
        /// </summary>
        protected abstract IMapFactory CellularAutomatonMapFactory { get; }

        /// <summary>
        /// Экземпляр фабрики, генерирующий карты на основе прямоугольных комнат.
        /// </summary>
        protected abstract IMapFactory RoomMapFactory { get; }

        protected abstract IMapFactory OpenMapFactory { get; }

        /// <summary>
        /// Возвращает генератор карты.
        /// </summary>
        /// <param name="sectorNode">
        /// Схема сектора, на основе которой будет принято решение,
        /// какой генератор карты использовать.
        /// </param>
        /// <returns> Возвращает фабрику карт для сектора. </returns>
        public IMapFactory GetMapFactory(ISectorNode sectorNode)
        {
            if (sectorNode is null)
            {
                throw new ArgumentNullException(nameof(sectorNode));
            }

            var sectorScheme = sectorNode.SectorScheme;

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

                case SchemeSectorMapGenerator.Open:
                    return OpenMapFactory;

                //TODO Прописать для всех схем конкретный генератор.
                // После явного прописывания здесь нужно будет выбрасывать исключение.
                default:
                    return RoomMapFactory;
            }
        }
    }
}