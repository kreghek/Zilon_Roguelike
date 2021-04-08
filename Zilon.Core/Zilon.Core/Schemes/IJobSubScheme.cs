using Zilon.Core.Skills;

namespace Zilon.Core.Schemes
{
    public interface IJobSubScheme : IMinimalJobSubScheme
    {
        string[]? Data { get; }
        JobScope Scope { get; }
        JobType Type { get; }
    }
}