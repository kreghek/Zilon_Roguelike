namespace Zilon.Core.Schemes
{
    public interface IPersonTemplateScheme : IScheme
    {
        IDropTableScheme? BodyEquipments { get; }
        string? FractionSid { get; }
        IDropTableScheme? HeadEquipments { get; }
        IDropTableScheme? InventoryProps { get; }
        IDropTableScheme? MainHandEquipments { get; }
        IDropTableScheme? OffHandEquipments { get; }
    }
}