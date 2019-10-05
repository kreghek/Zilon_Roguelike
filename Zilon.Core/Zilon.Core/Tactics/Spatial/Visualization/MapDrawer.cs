using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.Common;

namespace Zilon.Core.Tactics.Spatial.Visualization
{
    public sealed class MapDrawer
    {
        private int cellSize = 10;

        public Bitmap DrawMap(IMap map)
        {
            var hexNodes = map.Nodes.OfType<HexNode>();

            var info = GetImageInfo(hexNodes);

            var bitmap = CreateBitmap(info);

            DrawAllNodes(hexNodes, bitmap, info);

            return bitmap;
        }

        private void DrawAllNodes(IEnumerable<HexNode> nodes, Bitmap bitmap, ImageInfo info)
        {
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.FillRectangle(Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);
                foreach (var node in nodes)
                {
                    var coords = HexHelper.ConvertToWorld(node.OffsetX, node.OffsetY);

                    var x = (coords[0] - info.LeftCoord) * cellSize;
                    var y = (coords[1] - info.BottomCoord) * cellSize;

                    graphics.FillEllipse(Brushes.White, x, y - cellSize, cellSize, cellSize);
                }
            }
        }

        private Bitmap CreateBitmap(ImageInfo info)
        {
            var xAxisDiff = info.RightCoord - info.LeftCoord;
            var width = (xAxisDiff + 1) * cellSize;
            var yAxisDiff = info.TopCoord - info.BottomCoord;
            var height = (yAxisDiff + 1) * cellSize;

            var bitmap = new Bitmap(width, height);

            return bitmap;
        }

        private static ImageInfo GetImageInfo(IEnumerable<HexNode> nodes)
        {
            var xAxisOrderedNode = nodes.OrderBy(x => x.OffsetX);
            var yAxisOrderedNode = nodes.OrderBy(x => x.OffsetY);

            var info = new ImageInfo();

            info.LeftCoord = xAxisOrderedNode.First().OffsetX;
            info.RightCoord = xAxisOrderedNode.Last().OffsetX;

            info.BottomCoord = xAxisOrderedNode.First().OffsetY;
            info.TopCoord = xAxisOrderedNode.Last().OffsetY;

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
