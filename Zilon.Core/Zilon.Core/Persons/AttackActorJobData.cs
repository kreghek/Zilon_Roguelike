using Newtonsoft.Json;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Структура для работы с данными работы по атаке актёра.
    /// </summary>
    /// <remarks>
    /// Одновременно можеть быть заполнено либо <see cref="WeaponTags" />, либо <see cref="MonsterTags" />.
    /// </remarks>
    public sealed class AttackActorJobData
    {
        /// <summary>
        /// Теги монстров, которые должны быть у атакованного.
        /// </summary>
        [JsonProperty]
        public string[] MonsterTags { get; private set; }

        /// <summary>
        /// Теги оружия, которым произведено действие, чтобы работа была засчитана.
        /// </summary>
        [JsonProperty]
        public string[] WeaponTags { get; private set; }
    }
}