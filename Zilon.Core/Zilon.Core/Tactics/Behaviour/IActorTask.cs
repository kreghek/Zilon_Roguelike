namespace Zilon.Core.Tactics.Behaviour
{
    public interface IActorTask
    {
        bool IsComplete { get; }

        int Cost { get; }

        void Execute();
    }
}