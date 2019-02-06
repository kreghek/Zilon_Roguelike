using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Реализация менеджера сектора.
    /// </summary>
    /// <seealso cref="ISectorManager" />
    public class SectorManager : ISectorManager
    {
        /// <summary>
        /// Текущий сектор.
        /// </summary>
        public ISector CurrentSector { get; private set; }

        /// <summary>
        /// Создаёт текущий сектор по указанному генератору и настройкам.
        /// </summary>
        /// <param name="generator">Генератор сектора.</param>
        /// <param name="options">Настройки генерации сектора.</param>
        public void CreateSector(ISectorProceduralGenerator generator, ISectorGeneratorOptions options)
        {
            CurrentSector = generator.Generate(options);
        }
    }
}
