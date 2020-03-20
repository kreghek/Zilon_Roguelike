namespace Zilon.Core.Commands
{
    public interface ICommandWrapper<TContext>: ICommand<TContext>
    {
        ICommand<TContext> UnderlyingCommand { get; }
    }
}