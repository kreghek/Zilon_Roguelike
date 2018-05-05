using Assets.Zilon.Scripts.Commands;

namespace Assets.Zilon.Scripts.Services
{
    interface ICommandManager<TCommand> where TCommand: ICommand
    {
        void Push(TCommand command);
        TCommand Pop();
    }
}
