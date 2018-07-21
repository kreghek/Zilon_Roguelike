namespace Zilon.Core.Utils.SourceSchemes
{
    /// <summary>
    /// Схема характиристик предмета, который можно экипировать на персонажа.
    /// </summary>
    public class PropEquipSubScheme
    {
        /// <summary>
        /// Мощь. Влияет на все характиристики предмета.
        /// </summary>
        public float Power { get; set; }

        /// <summary>
        /// Ранг пробития брони.
        /// </summary>
        public int ApRank { get; set; }

        /// <summary>
        /// Ранг брони.
        /// </summary>
        public int ArmorRank { get; set; }

        /// <summary>
        /// Доля поглощения урона при равном ранге пробития и брони.
        /// </summary>
        /// <remarks>
        /// Зависит от Мощи.
        /// </remarks>
        public float Absorbtion { get; set; }

        /// <summary>
        /// Идентификаторы действий, которые позволяет совершать предмет.
        /// </summary>
        public string[] ActSids { get; set; }
    }
}
