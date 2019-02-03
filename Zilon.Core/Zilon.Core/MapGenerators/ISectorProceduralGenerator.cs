using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Генератор сектора.
    /// </summary>
    public interface ISectorProceduralGenerator
    {
        /// <summary> Создаёт сектора с учётом указанных настроек. </summary>
        /// <param name="options"> Настройки создания сектора. </param>
        /// <returns> Возвращает созданный сектор. </returns>
        ISector Generate(ISectorGeneratorOptions options);
    }
}