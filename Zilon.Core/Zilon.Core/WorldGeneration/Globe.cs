using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Мир.
    /// </summary>
    public class Globe
    {
        public TerrainCell[][] Terrain { get; set; }
        public List<Realm> Realms = new List<Realm>();

        /// <summary>
        /// Список основных городов в мире.
        /// </summary>
        public List<Locality> Localities = new List<Locality>();
        public List<Agent> Agents = new List<Agent>();
        public Dictionary<TerrainCell, List<Agent>> AgentCells = new Dictionary<TerrainCell, List<Agent>>();
        public Dictionary<TerrainCell, Locality> LocalitiesCells = new Dictionary<TerrainCell, Locality>();

        public ScanResult ScanResult = new ScanResult();

        public int AgentCrisys = 0;

        public TerrainCell StartProvince { get; set; }

        public TerrainCell HomeProvince { get; set; }

        public void Save(string path)
        {
            var branchColors = new[] { Color.Red, Color.Blue, Color.Green, Color.Yellow,
                Color.Black, Color.Magenta, Color.Maroon, Color.LightGray };
            using (var realmBmp = new DirectBitmap(Terrain.Length, Terrain[0].Length))
            using (var branchmBmp = new DirectBitmap(Terrain.Length, Terrain[0].Length))
            {
                for (var i = 0; i < Terrain.Length; i++)
                {
                    for (var j = 0; j < Terrain[0].Length; j++)
                    {
                        var cell = Terrain[i][j];
                        if (LocalitiesCells.TryGetValue(cell, out var locality))
                        {
                            var branch = locality.Branches.Single(x => x.Value > 0);

                            var owner = locality.Owner;
                            realmBmp.SetPixel(i, j, owner.Color);
                            branchmBmp.SetPixel(i, j, branchColors[(int)branch.Key]);
                        }
                        else
                        {
                            realmBmp.SetPixel(i, j, Color.White);
                            branchmBmp.SetPixel(i, j, Color.White);
                        }
                    }
                }

                realmBmp.Bitmap.Save(Path.Combine(path, "realms.bmp"), ImageFormat.Bmp);
                branchmBmp.Bitmap.Save(Path.Combine(path, "branches.bmp"), ImageFormat.Bmp);
            }
        }
    }
}
