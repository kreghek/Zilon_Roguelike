namespace Zilon.Core.Schemes
{
    public interface IJobSubScheme
    {
        string[] Data { get; }
        JobScope Scope { get; }
        JobType Type { get; }
        int Value { get; }
    }
}