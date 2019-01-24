using System.Collections.Generic;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration
{
    public class Generator
    {
        private readonly IDice _dice;

        public Generator(IDice dice)
        {
            _dice = dice;
        }

        public void Generate()
        {
            var realms = new Realm[4];
            for (var i = 0; i < 4; i++)
            {
                realms[i] = new Realm
                {

                };
            }


            var globe = new Globe
            {
                Terrain = new TerrainCell[100][]
            };

            var localities = new List<Locality>();
            for (var i = 0; i < 100; i++)
            {
                globe.Terrain[i] = new TerrainCell[100];

                for (var j = 0; j < 100; j++)
                {
                    globe.Terrain[i][j] = new TerrainCell();

                    if (_dice.Roll(100) > 75)
                    {
                        var rolledRealmIndex = _dice.Roll(0, 3);

                        var locality = new Locality()
                        {
                            Cells = new[] { globe.Terrain[i][j] },
                            Owner = realms[rolledRealmIndex]
                        };

                        localities.Add(locality);
                    }
                }
            }

            var agents = new List<Agent>();
            for (var i = 0; i < 40; i++)
            {
                var rolledLocalityIndex = _dice.Roll(0, localities.Count - 1);
                var locality = localities[rolledLocalityIndex];

                var agent = new Agent
                {
                    Name = $"agent {i}",
                    Localtion = locality.Cells[0],
                    Realm = locality.Owner
                };

                agents.Add(agent);
            }

            // обработка итераций
            for (var i = 0; i < 40_000; i++)
            {
                foreach (var agent in agents.ToArray())
                {
                    var rolledActionIndex = _dice.Roll(10);
                }
            }
        }
    }
}
