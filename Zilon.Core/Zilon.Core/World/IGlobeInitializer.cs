namespace Zilon.Core.World
{
    /// <summary>
    /// Create and initialize globe.
    /// </summary>
    public interface IGlobeInitializer
    {
        Task<IGlobe> CreateGlobeAsync(string startLocationSchemeSid);
    }
}