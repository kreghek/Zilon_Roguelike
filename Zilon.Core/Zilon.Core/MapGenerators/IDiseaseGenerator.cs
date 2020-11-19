using Zilon.Core.Diseases;

namespace Zilon.Core.MapGenerators
{
    public interface IDiseaseGenerator
    {
        IDisease Create();
    }
}