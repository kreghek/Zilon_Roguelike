using System;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public static class MapMetrics
    {
        public const int UNIT_SIZE = 32;
        public static float UnitSize => UNIT_SIZE; //(float)((2 * UNIT_SIZE) / Math.Sqrt(3));
    }
}