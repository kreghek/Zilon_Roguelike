using System.Threading.Tasks;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MassSectorGenerator
{
    internal class ImageHelper
    {
        private static void SaveMapAsImage(ISectorMap map, string outputPath)
        {
            using var bmp = MapDrawer.DrawMap(map);
            bmp.Save(outputPath);
        }

        public static Task SaveMapAsImageAsync(string outputPath, ISector sector)
        {
            if (outputPath != null)
            {
                SaveMapAsImage(sector.Map, outputPath);
            }

            return Task.CompletedTask;
        }
    }
}