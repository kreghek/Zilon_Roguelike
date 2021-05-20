using System.Threading;
using System.Threading.Tasks;

namespace Zilon.Core.Common
{
    public interface IReceiver<T>
    {
        void CancelReceiving();
        Task<T> ReceiveAsync(CancellationToken cancellationToken);
    }
}