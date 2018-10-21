using JetBrains.Annotations;

namespace Zilon.Core.Commands
{
    [PublicAPI]
    public interface ICommandManager
    {
        void Push(ICommand command);
        ICommand Pop();
    }
}
