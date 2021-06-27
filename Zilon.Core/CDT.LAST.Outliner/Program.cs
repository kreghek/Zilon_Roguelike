using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Zilon.CommonUtilities;

namespace CDT.LAST.Outliner
{
    internal class Program
    {
        private static bool CheckIsBound(Bitmap sourceBmp, int x, int y)
        {
            var offsets = new[] {
                new { x = 1, y = 0 },
                new { x = -1, y = 0 },
                new { x = 0, y = 1 },
                new { x = 0, y = -1 }
            };

            foreach (var offset in offsets)
            {
                var targetX = x + offset.x;
                var targetY = y + offset.y;

                if (targetX < 0 || targetY < 0 || targetX >= sourceBmp.Width || targetY >= sourceBmp.Height)
                {
                    continue;
                }

                var pixel = sourceBmp.GetPixel(targetX, targetY);
                if (pixel.A > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static void Main(string[] args)
        {
            var imagePath = ArgumentHelper.GetProgramArgument(args, "imagePath");
            var outputPath = ArgumentHelper.GetProgramArgument(args, "outputPath");

            var files = Directory.GetFiles(imagePath, "*.png");
            foreach (var file in files)
            {
                HandleFile(outputPath, file);
            }
        }

        private static void HandleFile(string outputPath, string file)
        {
            using var sourceBmp = new Bitmap(file);
            using var outputBmp = new Bitmap(sourceBmp.Width, sourceBmp.Height);
            for (var x = 0; x < sourceBmp.Width; x++)
            {
                for (var y = 0; y < sourceBmp.Height; y++)
                {
                    var currentPixel = sourceBmp.GetPixel(x, y);
                    if (currentPixel.A > 0)
                    {
                        outputBmp.SetPixel(x, y, Color.White);
                        continue;
                    }

                    var isBound = CheckIsBound(sourceBmp, x, y);

                    if (isBound)
                    {
                        outputBmp.SetPixel(x, y, Color.White);
                    }
                }
            }

            var outFile = Path.GetFileNameWithoutExtension(file);
            outputBmp.Save(Path.Combine(outputPath, outFile + ".png"), ImageFormat.Png);
        }
    }
}