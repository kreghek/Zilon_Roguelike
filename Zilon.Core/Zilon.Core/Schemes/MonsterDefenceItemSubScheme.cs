using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    public class MonsterDefenceItemSubScheme : IMonsterDefenceItemSubScheme
    {
        public MonsterDefenceItemSubScheme(DefenceType type, PersonRuleLevel level)
        {
            Type = type;
            Level = level;
        }

        public DefenceType Type { get; }
        public PersonRuleLevel Level { get; }
    }
}
