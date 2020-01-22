namespace Zilon.Core.World
{
    /// <summary>
    /// Интерфейс сущности в мире.
    /// Суда относятся:
    /// Армии,
    /// Миграционные группы,
    /// Бандиты,
    /// Экспедиции,
    /// Стаи животных,
    /// Великие монстры.
    /// </summary>
    public interface IGlobeEntity
    {
        /// <summary>
        /// Текущее местоположение сущности.
        /// </summary>
        TerrainCell Location { get; set; }
    }
}
