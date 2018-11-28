namespace Zilon.Core.Tactics.Behaviour
{
    public interface IActorTask
    {
        bool CanExecute();
        void Execute();
        bool IsComplete { get; }
    }
}