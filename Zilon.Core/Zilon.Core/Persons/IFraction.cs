namespace Zilon.Core.Persons
{
    public interface IFraction
    {
        string Name { get; }

        FractionRelation GetRelation(IFraction targetFraction);
    }
}