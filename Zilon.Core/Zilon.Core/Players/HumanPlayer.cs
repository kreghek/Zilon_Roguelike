using System;

using Zilon.Core.Persons;
using Zilon.Core.World;

namespace Zilon.Core.Players
{
    /// <summary>
    /// Класс игрока. Содержат данные игрока, переходящие между глобальной и локальной картами.
    /// </summary>
    /// <seealso cref="PlayerBase" />
    public class HumanPlayer : PlayerBase
    {
        private ProvinceNode _globeNode;

        /// <summary>
        /// Текущая провинция группы игрока.
        /// </summary>
        public TerrainCell Terrain { get; set; }

        /// <summary>
        /// Текущая локация группы игрока. Узел провинции.
        /// </summary>
        public ProvinceNode GlobeNode
        {
            get => _globeNode;
            set
            {
                _globeNode = value;
                GlobeNodeChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Ссылка на основного персонажа игрока.
        /// </summary>
        public HumanPerson MainPerson { get; set; }

        /// <summary> 
        /// Текущий идентфиикатор сектора внутри локации, где сейчас находится группа игрока.
        /// </summary>
        public string SectorSid { get; set; }

        /// <summary>
        /// Событие выстреливает, если зменяется узел группы игрока на глобальной карте.
        /// </summary>
        public event EventHandler GlobeNodeChanged;

        /// <summary>
        /// Сохранение текущего состояния группы игрока.
        /// </summary>
        /// <remarks>
        /// Сохраняет:
        /// 1. Состояние персонажа игрока.
        /// 2. Текущее положение группы игрока.
        /// </remarks>
        public void Save()
        {
            
        }
    }
}
