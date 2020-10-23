﻿namespace Zilon.Core.Persons
{
    public static class Fractions
    {
        static Fractions()
        {
            MonsterFraction = new Fraction("Monsters");
            MainPersonFraction = new Fraction("Main Hero");
        }

        public static IFraction MonsterFraction { get; private set; }

        public static IFraction MainPersonFraction { get; private set; }
    }
}