namespace Zilon.Core.Common
{
    public static class HexHelper
    {
        public static CubeCoords ConvertToCube(int offsetX, int offsetY)
        {
            var x = offsetX - (offsetY - (offsetY % 2)) / 2;
            var z = offsetY;
            var y = -x - z;

            return new CubeCoords(x, y, z);
        }

        public static float[] ConvertToWorld(int offsetX, int offsetY)
        {
            var rowOffset = offsetY % 2 == 0 ? 0 : 0.5f;
            return new[] {
                offsetX + rowOffset,
                offsetY * 3f / 4
            };
        }
    }
}
