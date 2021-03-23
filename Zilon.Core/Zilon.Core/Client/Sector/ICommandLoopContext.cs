using System.Threading;
using System.Threading.Tasks;

namespace Zilon.Core.Client.Sector
{
    public interface ICommandLoopContext
    { 
        bool HasNextIteration { get; }

        Task WaitForGlobeUpdate(CancellationToken cancellationToken);
    }
}