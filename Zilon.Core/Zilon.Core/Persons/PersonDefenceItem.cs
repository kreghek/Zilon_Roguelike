using Zilon.Core.Components;

namespace Zilon.Core.Persons
{
    public class PersonDefenceItem
    {
        public PersonDefenceItem(DefenceType type, PersonRuleLevel level)
        {
            Type = type;
            Level = level;
        }

        public PersonRuleLevel Level { get; }

        public DefenceType Type { get; }

        public override string ToString()
        {
            return $"{Type} {Level}";
        }
    }
}