using System.Threading.Tasks;

namespace Zilon.Core.WorldGeneration
{
    public sealed class TerrainInitiator
    {
        private const int WORLD_SIZE = 40;

        public Task<TerrainCell[][]> GenerateAsync()
        {
            return Task.Run(()=> {
                var terrain = new TerrainCell[WORLD_SIZE][];
                for (var i = 0; i < WORLD_SIZE; i++)
                {
                    terrain[i] = new TerrainCell[WORLD_SIZE];

                    for (var j = 0; j < WORLD_SIZE; j++)
                    {
                        terrain[i][j] = new TerrainCell
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
