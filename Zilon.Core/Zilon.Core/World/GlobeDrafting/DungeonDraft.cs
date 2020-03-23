namespace Zilon.Core.World.GlobeDrafting
{
    /// <summary>
    /// Черновик первоначального подземелья.
    /// </summary>
    public sealed class DungeonDraft
    {
        /// <summary>
        /// Стартовые координаты подземелья в координатах мира.
        /// </summary>
        public OffsetCoords StartTerrainCoords { get; set; }

        /// <summary>
        /// Схема стартового подземелья.
        /// </summary>
        public string SchemeSid { get; set; }

        /// <summary>
        /// Схема уровня, с которого начинается стартовое подземелье.
        /// </summary>
        public string SchemeLevelSid { get; set; }
    }
}
