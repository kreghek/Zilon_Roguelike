namespace Zilon.Core.World
{
    public class Color
    {
        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public byte R { get; }

        public byte G { get; }

        public byte B { get; }

        public static Color Red => new Color(255, 0, 0);
        public static Color Green => new Color(0, 255, 0);
        public static Color Blue => new Color(0, 0, 255);
        public static Color Yellow => new Color(255, 255, 0);
        public static Color Magenta => new Color(255, 0, 255);
        public static Color Cyan => new Color(0, 255, 255);
        public static Color LightGray => new Color(211, 211, 211);
        public static Color Beige => new Color(16, 119, 220);
    }
}
