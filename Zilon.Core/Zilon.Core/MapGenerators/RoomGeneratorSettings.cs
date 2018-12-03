namespace Zilon.Core.MapGenerators
{
    public sealed class RoomGeneratorSettings
    {
        public int RoomCount { get; set; }
        public int RoomCellSize { get; set; }
        public int MaxNeighbors { get; set; }
        public int NeighborProbably { get; set; }

        public RoomGeneratorSettings()
        {
            RoomCount = 20;
            RoomCellSize = 20;
            MaxNeighbors = 2;
            NeighborProbably = 100;
        }
    }
}
