using System.Threading.Tasks;

using Zilon.Core.World;

namespace Zilon.Core.WorldGeneration
{
    public sealed class TerrainInitiator
    {
        private const int WORLD_SIZE = 40;

        public Task<Terrain> GenerateAsync()
        {
            return Task.Run(() =>
            {
                var terrain = new Terrain
                {
                    Cells = new TerrainCell[WORLD_SIZE][]
                };

                for (var i = 0; i < WORLD_SIZE; i++)
                {
                    terrain.Cells[i] = new TerrainCell[WORLD_SIZE];

                    for (var j = 0; j < WORLD_SIZE; j++)
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
