namespace Zilon.Core.MapGenerators
{
    public interface IStaticObstaclesGenerator
    {
        Task CreateAsync(IStaticObjectGenerationContext generationContext);
    }
}