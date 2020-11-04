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
        }

        public static IFraction MonsterFraction { get; private set; }

        public static IFraction MainPersonFraction { get; private set; }

        public static IFraction InterventionistFraction { get; private set; }

        public static IFraction MilitiaFraction { get; private set; }
    }
}