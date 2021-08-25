using System.Threading.Tasks;

namespace Zilon.Core.MapGenerators
{
    public interface IStaticObjectsGenerator
    {
        Task CreateAsync(IStaticObjectGenerationContext generationContext);
    }
}