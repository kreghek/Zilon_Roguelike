namespace Zilon.Core.Commands
{
    public interface ICommandFactory
    {
        ICommand CreateCommand<T>() where T : class, ICommand;
    }
}
