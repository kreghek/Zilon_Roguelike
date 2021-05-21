using System;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public static class MapMetrics
    {
        public const int UNIT_SIZE = 32;
        public static float UnitSize => (float)((UNIT_SIZE * 2) / Math.Sqrt(3));
    }
}
