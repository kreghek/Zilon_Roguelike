namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Интерфейс схемы для одного типа обороны монстра.
    /// </summary>
    public interface IMonsterDefenceScheme: IScheme
    {
        IMonsterDefenceItemSubScheme[] Defences { get; }
    }
}
