namespace Zilon.Core.Client.Sector
{
    public interface ICommandLoopContext
    {
        bool CanPlayerGiveCommand { get; }
        bool HasNextIteration { get; }
    }
}