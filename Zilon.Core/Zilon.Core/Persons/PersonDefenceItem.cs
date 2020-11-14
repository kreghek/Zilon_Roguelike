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

        public DefenceType Type { get; }

        public PersonRuleLevel Level { get; }

        public override string ToString()
        {
            return $"{Type} {Level}";
        }
    }
}