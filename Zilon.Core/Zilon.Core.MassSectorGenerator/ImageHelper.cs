using System.Threading.Tasks;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MassSectorGenerator
{
    internal static class ImageHelper
    {
        public static Task SaveMapAsImageAsync(string outputPath, ISector sector)
        {
            if (outputPath is null)
            {
                throw new ArgumentNullException(nameof(outputPath));
            }

            return Task.Run(() => SaveMapAsImage(sector.Map, outputPath));
        }

        private static void SaveMapAsImage(ISectorMap map, string outputPath)
        {
            using var bmp = MapDrawer.DrawMap(map);
            bmp.Save(outputPath);
        }
    }
}