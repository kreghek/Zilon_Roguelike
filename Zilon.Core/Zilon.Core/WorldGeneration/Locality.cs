using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Город.
    /// </summary>
    public class Locality
    {
        public Locality()
        {
            CurrentEvents = new List<LocalityEvent>();
        }

        public string Name { get; set; }

        public TerrainCell Cell { get; set; }

        public Realm Owner { get; set; }

        public Dictionary<BranchType, int> Branches { get; set; }

        public int Population { get; set; }

        /// <summary>
        /// Счётчик мора в городе.
        /// </summary>
        public int FamineCounter { get; set; }

        /// <summary>
        /// Уровень криминала в городе. При сильно высоких значениях криминала из города начнёт уходить население.
        /// В режиме приключений в районе этого города будут часто встречаться бандиты.
        /// </summary>
        public int CriminalLevel { get; set; }

        /// <summary>
        /// Текущая еда в городе. При достижении нулевого значения в городе начнётся мор.
        /// В режиме приключений в городе не будут продавать еду.
        /// </summary>
        public int Food { get; set; }

        /// <summary>
        /// Энергия в городе. При нулевом значении в городе перестают работать часть предприятий.
        /// Городе снижает производство товаров для населения.
        /// В режиме приключений городе не будут продавать товары предприятий, которые обесточены.
        /// Если обесточены продуктовые предприятия, город начинает меньше производить еды. Что может стать причиной голода.
        /// </summary>
        public int Energy { get; set; }

        /// <summary>
        /// Обеспечение начеления города товарами (не едой). При достижении ключевого значения в городе начинается дефицит.
        /// В режиме приключений в городе не будут продавать определённые товары.
        /// </summary>
        public int Goods { get; set; }

        /// <summary>
        /// Текущие события в городе.
        /// </summary>
        public List<LocalityEvent> CurrentEvents { get; }

        public override string ToString()
        {
            return $"{Name} [{Owner}] ({Branches.First().Key})";
        }
    }
}
