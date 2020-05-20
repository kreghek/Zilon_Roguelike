using System.Threading.Tasks;

namespace Assets.Zilon.Scripts.Services
{
    interface ICommandBlockerService
    {
        void AddBlocker(ICommandBlocker commandBlocker);

        Task WaitBlockers();

        bool HasBlockers { get; }

        void DropBlockers();
    }
}
