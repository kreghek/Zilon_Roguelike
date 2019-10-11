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

        public static Bitmap DrawMap(IMap map)
        {
            var hexNodes = map.Nodes.OfType<HexNode>();

            var info = GetImageInfo(hexNodes);

            var bitmap = CreateBitmap(info);

            DrawAllNodes(hexNodes, bitmap, info);

            return bitmap;
        }

        private static void DrawAllNodes(IEnumerable<HexNode> nodes, Bitmap bitmap, ImageInfo info)
        {
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.FillRectangle(Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);
                foreach (var node in nodes)
                {
                    var coords = HexHelper.ConvertToWorld(node.OffsetX, node.OffsetY);

                    var x = (coords[0] - info.LeftCoord) * CELLSIZE;
                    var y = (coords[1] - info.BottomCoord) * CELLSIZE;

                    graphics.FillEllipse(Brushes.White, x + CELLSIZE, y + CELLSIZE, CELLSIZE, CELLSIZE);
                }
            }
        }

        private static Bitmap CreateBitmap(ImageInfo info)
        {
            var xAxisDiff = info.RightCoord - info.LeftCoord;
            var width = (xAxisDiff + 1) * CELLSIZE;
            var yAxisDiff = info.TopCoord - info.BottomCoord;
            var height = (yAxisDiff + 1) * CELLSIZE;

            var margin = CELLSIZE * 2;
            var bitmap = new Bitmap(width + margin, height + margin);

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
