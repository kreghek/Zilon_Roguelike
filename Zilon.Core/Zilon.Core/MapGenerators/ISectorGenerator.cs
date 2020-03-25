using System.Threading.Tasks;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Генератор секторов разного типа.
    /// </summary>
    public interface ISectorGenerator
    {
        /// <summary> Создаёт сектор подземелья с учётом указанных настроек. </summary>
        /// <param name="sectorScheme"> Схема создания сектора. </param>
        /// <returns> Возвращает созданный сектор. </returns>
        Task<ISector> GenerateAsync(ISectorSubScheme sectorScheme);
    }
}