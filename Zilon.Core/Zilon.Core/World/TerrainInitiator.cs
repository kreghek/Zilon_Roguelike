using System.Threading.Tasks;

namespace Zilon.Core.World
{
    public sealed class TerrainInitiator
    {
        public int WorldSize { get; } = 40;

        public Task<Terrain> GenerateAsync()
        {
            return Task.Run(() =>
            {
                var terrain = new Terrain
                {
                    Cells = new TerrainCell[WorldSize][]
                };

                for (var i = 0; i < WorldSize; i++)
                {
                    terrain.Cells[i] = new TerrainCell[WorldSize];

                    for (var j = 0; j < WorldSize; j++)
                    {
                        terrain.Cells[i][j] = new TerrainCell
                        {
                            Coords = new OffsetCoords(i, j)
                        };
                    }
                }

                return terrain;
            });
        }
    }
}
