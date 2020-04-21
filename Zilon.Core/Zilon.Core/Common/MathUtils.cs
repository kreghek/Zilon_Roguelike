using System;
using System.Collections.Generic;
using System.Text;

namespace Zilon.Core.Common
{
    public static class MathUtils
    {
        public static float Lerp(int a, int b, float t)
        {
            return a + (b - a) * t;
        }
    }
}
