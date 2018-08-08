using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Zilon.Core.Schemes
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum JobType
    {
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
        Craft
    }
}
