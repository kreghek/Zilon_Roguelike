namespace Zilon.Core.Schemes
{
    public interface IPropScheme: IScheme
    {
        CraftSubScheme Craft { get; }
        IPropEquipSubScheme Equip { get; set; }
        IPropUseSubScheme Use { get; set; }
    }
}