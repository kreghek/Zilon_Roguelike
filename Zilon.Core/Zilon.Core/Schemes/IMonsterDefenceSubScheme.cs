namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Интерфейс схемы для одного типа обороны монстра.
    /// </summary>
    public interface IMonsterDefenceSubScheme
    {
        IMonsterDefenceItemSubScheme[] Defences { get; }
    }
}
