using System.Linq;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public class MonsterDefenceSubScheme : IMonsterDefenseSubScheme
    {
        [JsonConstructor]
        public MonsterDefenceSubScheme([CanBeNull] [ItemNotNull] MonsterDefenceItemSubScheme[] defences)
        {
            Defenses = defences?.Cast<IMonsterDefenceItemSubScheme>().ToArray();
        }

        public IMonsterDefenceItemSubScheme[] Defenses { get; }
    }
}
