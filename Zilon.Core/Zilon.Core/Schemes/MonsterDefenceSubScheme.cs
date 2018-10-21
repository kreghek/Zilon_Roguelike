using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public class MonsterDefenceSubScheme : IMonsterDefenseSubScheme
    {
        [JsonConstructor]
        public MonsterDefenceSubScheme([CanBeNull] MonsterDefenceItemSubScheme[] defences)
        {
            // ReSharper disable once CoVariantArrayConversion
            Defenses = defences;
        }

        public IMonsterDefenceItemSubScheme[] Defenses { get; }
    }
}
