using System;

using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Фракция персонажа.
    /// </summary>
    //TODO Отказаться от этого энума в пользу фракции, как схемы.
    [Flags]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum PersonFaction
    {
        Neutral = 0,
        Legion = 1 << 0,
        Gaarns = 1 << 1,
        Aleberts = 1 << 2,
        Deamons = 1 << 3,
        Cult = 1 << 4,
        Techbots = 1 << 5,
        Other = 1 << 6,
    }
}
