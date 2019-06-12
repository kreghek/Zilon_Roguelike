namespace Zilon.Core.Common
{
    /// <summary>
    /// Классификация монстров по тэгам.
    /// </summary>
    public static class MobTags
    {
        /// <summary>
        /// Типы монстров.
        /// </summary>
        public static class Type
        {
            /// <summary>
            /// Животные.
            /// </summary>
            public const string Animals = "animals";

            /// <summary>
            /// Боссы.
            /// </summary>
            public const string Boss = "boss";

            /// <summary>
            /// Демоны.
            /// </summary>
            public const string Demon = "demon";

            /// <summary>
            /// Летающие.
            /// </summary>
            public const string Fly = "fly";

            /// <summary>
            /// Сундук.
            /// </summary>
            public const string Luggage = "luggage";

            /// <summary>
            /// Нематериальные.
            /// </summary>
            public const string Wisp = "wisp";

            /// <summary>
            /// Крысы.
            /// </summary>
            public static class Rat
            {
                /// <summary>
                /// Трусливые крысы.
                /// </summary>
                public const string Cowardly = "cowardly-rat";

                /// <summary>
                /// Адские крысы.
                /// </summary>
                public const string Hell = "hell-rat";

                /// <summary>
                /// Лунные крысы.
                /// </summary>
                public const string Moon = "moon-rat";

                /// <summary>
                /// Крысы-мутанты.
                /// </summary>
                public const string Mutant = "mutant-rat";
            }
        }
    }
}