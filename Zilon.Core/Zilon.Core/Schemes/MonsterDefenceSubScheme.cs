using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public class MonsterDefenceSubScheme : IMonsterDefenceSubScheme
    {
        [JsonConstructor]
        public MonsterDefenceSubScheme([CanBeNull] MonsterDefenceItemSubScheme[] defences)
        {
            // ReSharper disable once CoVariantArrayConversion
            Defences = defences;
        }

        public IMonsterDefenceItemSubScheme[] Defences { get; }
    }
}
