namespace Zilon.Core.Commands
{
    public interface ICommandWrapper: ICommand
    {
        ICommand UnderlyingCommand { get; }
    }
}