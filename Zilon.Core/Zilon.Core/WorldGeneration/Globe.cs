using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Мир.
    /// </summary>
    public class Globe
    {
        public TerrainCell[][] Terrain { get; set; }
        public List<Realm> Realms = new List<Realm>();
        public List<Locality> Localities = new List<Locality>();
        public List<Agent> Agents = new List<Agent>();
        public Dictionary<TerrainCell, List<Agent>> AgentCells = new Dictionary<TerrainCell, List<Agent>>();
        public Dictionary<TerrainCell, Locality> LocalitiesCells = new Dictionary<TerrainCell, Locality>();

        public ScanResult ScanResult = new ScanResult();

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

    public class ScanResult
    {
        public HashSet<TerrainCell> Free = new HashSet<TerrainCell>();
    }

    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public int[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new int[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, Color colour)
        {
            int index = x + (y * Width);
            int col = colour.ToArgb();

            Bits[index] = col;
        }

        public Color GetPixel(int x, int y)
        {
            int index = x + (y * Width);
            int col = Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        public void Dispose()
        {
            if (Disposed)
            {
                return;
            }

            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }
}
