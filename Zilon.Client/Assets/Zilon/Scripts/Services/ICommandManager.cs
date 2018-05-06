using Assets.Zilon.Scripts.Models.Commands;

namespace Assets.Zilon.Scripts.Services
{
    interface ICommandManager
    {
        void Push(ICommand command);
        ICommand Pop();
    }
}
