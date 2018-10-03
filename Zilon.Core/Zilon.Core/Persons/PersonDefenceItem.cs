using Zilon.Core.Components;

namespace Zilon.Core.Persons
{
    public class PersonDefenceItem
    {
        public DefenceType Type { get; }

        public PersonRuleLevel Level { get; }

        public PersonDefenceItem(DefenceType type, PersonRuleLevel level)
        {
            Type = type;
            Level = level;
        }
    }
}
