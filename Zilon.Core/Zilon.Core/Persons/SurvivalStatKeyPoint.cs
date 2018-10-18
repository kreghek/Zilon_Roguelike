namespace Zilon.Core.Persons
{
    public class SurvivalStatKeyPoint
    {
        public SurvivalStatKeyPoint(SurvivalStatHazardLevel level, int value)
        {
            Level = level;
            Value = value;
        }

        public SurvivalStatHazardLevel Level { get; }
        public int Value { get; }

        public override string ToString()
        {
            return $"{Level}: {Value}";
        }
    }
}
