namespace Zilon.Core.Schemes
{
    /// <summary>
    ///     Доступные крафтовые компетенции.
    /// </summary>
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum ProfessionType
    {
        Сarpenter,
        WoodProcessing,
        MetalProcessing,
        PlasticProcessing,
        Mechanics,
        Armory,
        Weaver,
        Frayer,
        Tanner,
        Blacksmith,
        Engineer,
        Chimestry,
        Radiotechnic,
        Demolisher
    }
}