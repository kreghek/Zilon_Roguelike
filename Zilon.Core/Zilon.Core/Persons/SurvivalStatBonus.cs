namespace Zilon.Core.Persons
{
    public sealed class SurvivalStatBonus
    {
        public SurvivalStatBonus(SurvivalStatType survivalStatType)
        {
            SurvivalStatType = survivalStatType;
        }

        public SurvivalStatType SurvivalStatType { get; }

        public float Bonus { get; set; }
    }
}
