using System;

using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Реализация селектора генератора сектора.
    /// </summary>
    public sealed class SectorGeneratorSelector : ISectorGeneratorSelector
    {
        private readonly ITownSectorGenerator _townSectorGenerator;
        private readonly IDungeonSectorGenerator _dungeonSectorGenerator;

        /// <summary>
        /// Конструктор реализации селектора.
        /// </summary>
        /// <param name="townSectorGenerator"> Генератор сектора для фрагмента города. </param>
        /// <param name="dungeonSectorGenerator"> Генератор сектора для подземелья. </param>
        public SectorGeneratorSelector(ITownSectorGenerator townSectorGenerator,
            IDungeonSectorGenerator dungeonSectorGenerator)
        {
            _townSectorGenerator = townSectorGenerator;
            _dungeonSectorGenerator = dungeonSectorGenerator;
        }

        /// <summary>
        /// Выбирает генератор сектора в зависимости от локации.
        /// </summary>
        /// <param name="globeNode"> Узел локации на графе провинции на глобальной карте. </param>
        /// <returns> Возвращает генератор сектора. </returns>
        public ISectorGenerator GetGenerator(GlobeRegionNode globeNode)
        {
            if (globeNode == null)
            {
                return _dungeonSectorGenerator;
            }

            if (globeNode.IsTown)
            {
                return _townSectorGenerator;
            }

            return _dungeonSectorGenerator;
        }
    }
}
