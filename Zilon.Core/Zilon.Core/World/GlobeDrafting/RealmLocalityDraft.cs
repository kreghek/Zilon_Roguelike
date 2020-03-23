namespace Zilon.Core.World.GlobeDrafting
{
    /// <summary>
    /// Черновое описание поселения ключевого государства.
    /// </summary>
    public class RealmLocalityDraft
    {
        /// <summary>
        /// Стартовые координаты поселения в координатах мира.
        /// </summary>
        public OffsetCoords StartTerrainCoords { get; set; }

        /// <summary>
        /// Начальная численность.
        /// </summary>
        public int Population { get; set; }
    }
}
