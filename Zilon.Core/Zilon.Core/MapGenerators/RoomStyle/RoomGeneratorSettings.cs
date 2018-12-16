using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Настройки для генерации карт из комнат.
    /// </summary>
    public sealed class RoomGeneratorSettings
    {
        public int RoomCount { get; set; }
        public int RoomCellSize { get; set; }
        public int MaxNeighbors { get; set; }

        [ExcludeFromCodeCoverage]
        public RoomGeneratorSettings()
        {
            RoomCount = 20;
            RoomCellSize = 20;
            MaxNeighbors = 1;
        }
    }
}
