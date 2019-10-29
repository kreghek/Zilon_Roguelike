using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MassSectorGenerator
{
    public static class MapDrawer
    {
        private const int CELLSIZE = 10;
        private const int MARGIN = 10;
        private const int AXIS_FONT_SIZE = 6;

        public static Bitmap DrawNodes(IEnumerable<HexNode> nodes)
        {
            var info = GetImageInfo(nodes);

            var bitmap = CreateBitmap(info);

            DrawAllNodes(nodes, bitmap, info);

            return bitmap;
        }

        public static Bitmap DrawMap(IMap map)
        {
            var hexNodes = map.Nodes.OfType<HexNode>().ToArray();

            var bitmap = DrawNodes(hexNodes);

            return bitmap;
        }

        private static void DrawAllNodes(IEnumerable<HexNode> nodes, Bitmap bitmap, ImageInfo info)
        {
            using (var graphics = Graphics.FromImage(bitmap))
            {
                Clear(bitmap, graphics);

                DrawAxisNumbers(info, graphics);

                foreach (var node in nodes)
                {
                    var coords = HexHelper.ConvertToWorld(node.OffsetX, node.OffsetY);

                    var x = (coords[0] - info.LeftCoord) * CELLSIZE;
                    var y = (coords[1] - info.BottomCoord) * CELLSIZE;

                    var cellBrush = !node.IsObstacle ? Brushes.White : Brushes.Gray;

                    graphics.FillEllipse(cellBrush, x + MARGIN, y + MARGIN, CELLSIZE, CELLSIZE);
                }
            }
        }

        private static void Clear(Bitmap bitmap, Graphics graphics)
        {
            graphics.FillRectangle(Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);
        }

        private static void DrawAxisNumbers(ImageInfo info, Graphics graphics)
        {
            using (var font = new Font(SystemFonts.DefaultFont.FontFamily, AXIS_FONT_SIZE, FontStyle.Regular))
            {
                var width = info.RightCoord - info.LeftCoord;
                for (var i = 0; i <= width; i++)
                {
                    // Приводим к float, чтобы избежать переполнения при умножении int.
                    // https://lgtm.com/rules/1506096756023/
                    var xNumberCoord = (float)i * CELLSIZE;
                    graphics.DrawString(i.ToString(), font, Brushes.White, xNumberCoord + MARGIN, 0);
                }

                var height = info.TopCoord - info.BottomCoord;
                for (var i = 0; i <= height; i++)
                {
                    var ratio = 3f / 4;
                    // Приводим к float, чтобы избежать переполнения при умножении int.
                    // Здесь от float мы всё равно не избавимся из-за ratio.
                    // https://lgtm.com/rules/1506096756023/
                    var yMarginCoord = (float)i * CELLSIZE * ratio;
                    graphics.DrawString(i.ToString(), font, Brushes.White, MARGIN, yMarginCoord + MARGIN);
                }
            }
        }

        private static Bitmap CreateBitmap(ImageInfo info)
        {
            var xAxisDiff = info.RightCoord - info.LeftCoord;
            var width = (xAxisDiff + 1) * CELLSIZE;
            var yAxisDiff = info.TopCoord - info.BottomCoord;
            var height = (yAxisDiff + 1) * CELLSIZE;

            var twoSideMargin = MARGIN * 2;
            var bitmap = new Bitmap(width + twoSideMargin, height + twoSideMargin);

            return bitmap;
        }

        private static ImageInfo GetImageInfo(IEnumerable<HexNode> nodes)
        {
            var xAxisOrderedNode = nodes.OrderBy(x => x.OffsetX);
            var yAxisOrderedNode = nodes.OrderBy(x => x.OffsetY);

            var info = new ImageInfo();

            info.LeftCoord = xAxisOrderedNode.First().OffsetX;
            info.RightCoord = xAxisOrderedNode.Last().OffsetX;

            info.BottomCoord = yAxisOrderedNode.First().OffsetY;
            info.TopCoord = yAxisOrderedNode.Last().OffsetY;

            return info;
        }

        private sealed class ImageInfo
        {
            public int LeftCoord { get; set; }
            public int RightCoord { get; set; }
            public int TopCoord { get; set; }
            public int BottomCoord { get; set; }
        }
    }
}
