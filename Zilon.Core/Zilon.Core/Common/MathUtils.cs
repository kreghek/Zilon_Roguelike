namespace Zilon.Core.Common
{
    /// <summary>
    ///     Вспомогательный класс для математических операций.
    /// </summary>
    public static class MathUtils
    {
        public static float Lerp(int a, int b, float t)
        {
            var length = b - a;
            float lengthShare = length * t;
            return a + lengthShare;
        }
    }
}