namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Интерфейс схемы для одного типа обороны монстра.
    /// </summary>
    public interface IMonsterDefenseSubScheme
    {
        IMonsterDefenceItemSubScheme[] Defenses { get; }
    }
}
