namespace Zilon.Core.Client.Sector
{
    public interface ICommandLoopContext
    {
        bool HasNextIteration { get; }

        bool CanPlayerGiveCommand { get; }
    }
}