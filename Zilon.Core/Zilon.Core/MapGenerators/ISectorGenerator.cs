using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Генератор секторов разного типа.
    /// </summary>
    public interface ISectorGenerator
    {
        /// <summary> Создаёт сектор подземелья с учётом указанных настроек. </summary>
        /// <param name="sectorNode"> Схема создания сектора. </param>
        /// <returns> Возвращает созданный сектор. </returns>
        Task<ISector> GenerateAsync(ISectorNode sectorNode);
    }
}