namespace Zilon.Core.Persons
{
    public sealed class Fraction : IFraction
    {
        public Fraction(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public FractionRelation GetRelation(IFraction targetFraction)
        {
            if ((this == Fractions.MonsterFraction) && (targetFraction != Fractions.MonsterFraction))
            {
                // Фракция монстров нападает на всех, кроме монстров.
                // У монстров нет друзей.
                return FractionRelation.Enmity;
            }

            if ((this != Fractions.MonsterFraction) && (targetFraction == Fractions.MonsterFraction))
            {
                // С монтсрами никто не дружит.
                // Все фракции считают их врагами.
                return FractionRelation.Enmity;
            }

            // Все фракции, кроме монстров, друг к другу относятся нейтрально.
            return FractionRelation.Neutral;
        }
    }
}