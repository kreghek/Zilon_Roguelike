namespace Zilon.Core.World
{
    public class BannerColor
    {
        public BannerColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public byte B { get; }

        public static BannerColor Beige => new BannerColor(16, 119, 220);

        public static BannerColor Blue => new BannerColor(0, 0, 255);

        public static BannerColor Cyan => new BannerColor(0, 255, 255);

        public byte G { get; }

        public static BannerColor Green => new BannerColor(0, 255, 0);

        public static BannerColor LightGray => new BannerColor(211, 211, 211);

        public static BannerColor Magenta => new BannerColor(255, 0, 255);

        public byte R { get; }

        public static BannerColor Red => new BannerColor(255, 0, 0);

        public static BannerColor Yellow => new BannerColor(255, 255, 0);
    }
}