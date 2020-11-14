using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public interface IPersonTemplateScheme : IScheme
    {
        string FractionSid { get; }
        IDropTableScheme HeadEquipments { get; }
        IDropTableScheme BodyEquipments { get; }
        IDropTableScheme MainHandEquipments { get; }
        IDropTableScheme OffHandEquipments { get; }
        IDropTableScheme InventoryProps { get; }
    }

    public sealed class PersonTemplateScheme : SchemeBase, IPersonTemplateScheme
    {
        [JsonProperty]
        [JsonConverter(typeof(ConcreteTypeConverter<DropTableScheme>))]
        public IDropTableScheme HeadEquipments { get; private set; }

        [JsonProperty]
        [JsonConverter(typeof(ConcreteTypeConverter<DropTableScheme>))]
        public IDropTableScheme BodyEquipments { get; private set; }

        [JsonProperty]
        [JsonConverter(typeof(ConcreteTypeConverter<DropTableScheme>))]
        public IDropTableScheme MainHandEquipments { get; private set; }

        [JsonProperty]
        [JsonConverter(typeof(ConcreteTypeConverter<DropTableScheme>))]
        public IDropTableScheme OffHandEquipments { get; private set; }

        [JsonProperty]
        [JsonConverter(typeof(ConcreteTypeConverter<DropTableScheme>))]
        public IDropTableScheme InventoryProps { get; private set; }

        [JsonProperty] public string FractionSid { get; private set; }
    }
}