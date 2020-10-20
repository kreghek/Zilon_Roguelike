namespace Zilon.Core.Persons
{
    public sealed class SurvivalStatBonus
    {
        public SurvivalStatBonus(SurvivalStatType survivalStatType)
        {
            SurvivalStatType = survivalStatType;
        }

        public SurvivalStatType SurvivalStatType { get; }

        public float ValueBonus { get; set; }

        public float DownPassBonus { get; set; }
    }
}