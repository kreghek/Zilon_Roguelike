using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Zilon.Core.Schemes
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum JobType
    {
        Undefined = 0,
        Defeats = 1,  // Просто повергнуть кого-нибудь. Все победы - это нанесение крита, дающего OutOfControl
        DefeatGaarns,  // повергнуть представителя народа Гаарн
        DefeatAleberts,  // повергнуть алеберта
        DefeatLegions,  // повергнуть легионера
        DefeatDeamons,  // повергнуть демона-наемника
        DefeatCults,  // повергнуть технокультиста
        DefeatTechbots,  // повергнуть робота-самурая
        Blocks,  // Выдержать удар
        Hits,  // Попасть
        Crits,  // Получить крит
        Combats,  // Поучавствовать в боях
        Victories,  // Победить
        ReceiveHits,
        ReceiveDamage,
        ReceiveDamagePercent,
        CraftAssaultRifle,
        MeleeHits,  // Попасть в рукопашном бою
        OneUseHits, // задамажить одним использованием скилла
        OneUseDefeats, // уничтожить одним использованием скилла
        DefeatClasses,  // Повергнуть персонажей указанных поколений,
        Craft,

        /// <summary>
        /// Поглотить провиант.
        /// </summary>
        ConsumeProviant,

        /// <summary>
        /// Атаковать актёра.
        /// </summary>
        /// <remarks>
        /// Атаковать - значит использовать действие с типом Attack, чтобы целью был другой актёр.
        /// Эти работы могут содержать данные, в которых указаны подробности, кого и как нужно атаковать.
        /// Например, атаковать монстра с тегом beast. Или атаковать с использованием оружия с тегом sword.
        /// Если данные не указаны, то засчитывается любая атака.
        /// </remarks>
        AttacksActor
    }
}
