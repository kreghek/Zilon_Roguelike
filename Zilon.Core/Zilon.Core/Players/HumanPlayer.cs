using Zilon.Core.Persons;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.Players
{
    /// <summary>
    /// Класс игрока. Содержат данные игрока, переходящие между глобальной и локальной картами.
    /// </summary>
    /// <seealso cref="PlayerBase" />
    public class HumanPlayer: PlayerBase
    {
        /// <summary>
        /// Текущая провинция группы игрока.
        /// </summary>
        public TerrainCell Terrain { get; set; }

        /// <summary>
        /// Текущая локация группы игрока. Узел провинции.
        /// </summary>
        public GlobeRegionNode GlobeNode { get; set; }

        /// <summary>
        /// Ссылка на основного персонажа игрока.
        /// </summary>
        public HumanPerson MainPerson { get; set; }

        /// <summary> 
        /// Текущий идентфиикатор сектора внутри локации, где сейчас находится группа игрока.
        /// </summary>
        public string SectorSid { get; set; }
    }
}
