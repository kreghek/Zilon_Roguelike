using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public class MonsterDefenceSubScheme : IMonsterDefenceSubScheme
    {
        [JsonConstructor]
        public MonsterDefenceSubScheme(MonsterDefenceItemSubScheme[] defences)
        {
            Defences = defences;
        }

        public IMonsterDefenceItemSubScheme[] Defences { get; }
    }
}
