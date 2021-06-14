using System.Threading;
using System.Threading.Tasks;

namespace Zilon.Core.Common
{
    public interface ISender<in T>
    {
        Task SendAsync(T obj, CancellationToken cancellationToken);
    }
}