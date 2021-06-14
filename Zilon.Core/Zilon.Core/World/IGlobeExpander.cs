using System.Threading.Tasks;

namespace Zilon.Core.World
{
    /// <summary>
    /// Интерфейс расширителя мира.
    /// Каждый раз, когда кто-нибудь доходит до края мира, мир будет расширяться.
    /// </summary>
    public interface IGlobeExpander
    {
        /// <summary>
        /// Расширение мира в указанной узл.
        /// В результате указанный узел будет материализован и иметь переходы в новые нематериализованные узлы.
        /// </summary>
        /// <param name="sectorNode"> Узел расширения. </param>
        Task ExpandAsync(ISectorNode sectorNode);
    }
}