namespace Zilon.Core.Tactics.Behaviour
{
    public interface IActorTask
    {
        IActor Actor { get; }
        void Execute();
        bool IsComplete { get; }
    }
}