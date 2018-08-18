namespace Zilon.Core.Persons
{
    public class SurvivalStatHazardEffect : IPersonEffect
    {
        public SurvivalStatHazardEffect(SurvivalStatType type, SurvivalStatHazardLevel level)
        {
            Type = type;
            Level = level;
        }

        public SurvivalStatType Type { get; }

        public SurvivalStatHazardLevel Level { get; set; }

        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}
