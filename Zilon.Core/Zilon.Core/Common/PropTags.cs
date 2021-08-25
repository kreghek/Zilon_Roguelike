﻿namespace Zilon.Core.Common
{
    /// <summary>
    /// Справочник тегов, которые могут быть у предметов.
    /// </summary>
    public static class PropTags
    {
        /// <summary>
        /// Теги, которые могут быть у экипировки.
        /// </summary>
        public static class Equipment
        {
            public static string Armor => "armor";

            /// <summary>
            /// Экипировка может использоваться, как оружие дальнего боя.
            /// </summary>
            public static string Ranged => "ranged";

            /// <summary>
            /// Экипировка может использоваться, как щит.
            /// </summary>
            public static string Shield => "shield";

            /// <summary>
            /// Экипировка может использоваться, как оружие.
            /// </summary>
            public static string Weapon => "weapon";

            public static class WeaponClass
            {
                public static string Axe => "axe";
                public static string Bow => "bow";
                public static string Mace => "mace";
                public static string Pistol => "pistol";
                public static string Pole => "pole";
                public static string Staff => "staff";
                public static string Sword => "sword";
                public static string Wand => "wand";
            }
        }
    }
}