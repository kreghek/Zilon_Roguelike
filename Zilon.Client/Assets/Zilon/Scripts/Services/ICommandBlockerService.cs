using System.Threading.Tasks;

namespace Assets.Zilon.Scripts.Services
{
    interface ICommandBlockerService
    {
        void AddBlocker(ICommandBlocker commandBlocker);

        Task WaitBlockersAsync();

        bool HasBlockers { get; }

        void DropBlockers();
    }
}
