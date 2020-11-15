namespace Zilon.Core.Persons
{
    public sealed class SurvivalStatBonus
    {
        public SurvivalStatBonus(SurvivalStatType survivalStatType)
        {
            SurvivalStatType = survivalStatType;
        }

        public float DownPassBonus { get; set; }

        public SurvivalStatType SurvivalStatType { get; }

        public float ValueBonus { get; set; }
    }
}