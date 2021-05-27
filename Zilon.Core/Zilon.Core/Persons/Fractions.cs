namespace Zilon.Core.Persons
{
    public static class Fractions
    {
        static Fractions()
        {
            MonsterFraction = new Fraction("Monsters");
            MainPersonFraction = new Fraction("Main Hero");
            InterventionistFraction = new Fraction("Interventionists");
            MilitiaFraction = new Fraction("Militia");
            TroublemakerFraction = new Fraction("Trublemakers");
            Pilgrims = new Fraction("Pilgrims");
        }

        public static IFraction InterventionistFraction { get; }

        public static IFraction MainPersonFraction { get; }

        public static IFraction MilitiaFraction { get; }

        public static IFraction MonsterFraction { get; }

        public static IFraction Pilgrims { get; }

        public static IFraction TroublemakerFraction { get; }
    }
}