namespace Zilon.Core.Tactics.Behaviour
{
    public interface IActorTask
    {
        int Cost { get; }

        bool IsComplete { get; }

        void Execute();
    }
}