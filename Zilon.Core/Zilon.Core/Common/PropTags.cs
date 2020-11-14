namespace Zilon.Core.Common
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
            /// <summary>
            /// Экипировка может использоваться, как оружие.
            /// </summary>
            public static string Weapon => "weapon";

            /// <summary>
            /// Экипировка может использоваться, как оружие дальнего боя.
            /// </summary>
            public static string Ranged => "ranged";

            /// <summary>
            /// Экипировка может использоваться, как щит.
            /// </summary>
            public static string Shield => "shield";
        }
    }
}