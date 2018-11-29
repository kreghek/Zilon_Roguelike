namespace Zilon.Core.Tactics.Behaviour
{
    public interface IActorTask
    {
        void Execute();
        bool IsComplete { get; }
    }
}