namespace Zilon.Core.Schemes
{
    public interface IPropScheme: IScheme
    {
        CraftSubScheme Craft { get; }
        IPropEquipSubScheme Equip { get; }
        IPropUseSubScheme Use { get; }
    }
}