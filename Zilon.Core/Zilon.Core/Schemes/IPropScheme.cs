namespace Zilon.Core.Schemes
{
    public interface IPropScheme: IScheme
    {
        string[] Tags { get; }
        CraftSubScheme Craft { get; }
        IPropEquipSubScheme Equip { get; }
        IPropUseSubScheme Use { get; }
        
        /// <summary>
        /// Информация о предмете, как он используется для выполнения действий.
        /// </summary>
        IPropBulletSubScheme Bullet { get; }
    }
}