namespace Zilon.Core.Commands
{
    public interface ICommandManager
    {
        void Push(ICommand command);
        ICommand Pop();
    }
}
