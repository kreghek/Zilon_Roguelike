using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public class MonsterDefenceSubScheme : IMonsterDefenseSubScheme
    {
        [JsonProperty]
        [JsonConverter(typeof(ConcreteTypeConverter<MonsterDefenceItemSubScheme[]>))]
        public IMonsterDefenceItemSubScheme[] Defenses { get; private set; }
    }
}