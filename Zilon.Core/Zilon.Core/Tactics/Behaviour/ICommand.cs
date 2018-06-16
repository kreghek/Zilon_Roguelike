namespace Zilon.Core.Tactics.Behaviour
{
    public interface ICommand
    {
        IActor Actor { get; }
        void Execute();
        bool IsComplete { get; }
    }
}