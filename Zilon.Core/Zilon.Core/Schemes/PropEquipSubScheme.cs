namespace Zilon.Core.Schemes
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
    }
}
