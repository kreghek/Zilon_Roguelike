using System;
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
            for (var x1 = -1; x1 <= 1; x1++)
            {
                for (var y1 = -1; y1 <= 1; y1++)
                {
                    var targetX = x + x1;
                    var targetY = y + y1;

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
                            for (var x1 = -1; x1 <= 1; x1++)
                            {
                                for (var y1 = -1; y1 <= 1; y1++)
                                {
                                    var targetX = x + x1;
                                    var targetY = y + y1;

                                    if (targetX < 0 || targetY < 0 || targetX >= sourceBmp.Width ||
                                        targetY >= sourceBmp.Height)
                                    {
                                        continue;
                                    }

                                    outputBmp.SetPixel(x + x1, y + y1, Color.White);
                                }
                            }
                        }
                    }
                }

                var outFile = Path.GetFileNameWithoutExtension(file);
                outputBmp.Save(Path.Combine(outputPath, outFile + ".png"), ImageFormat.Png);
            }
        }
    }
}