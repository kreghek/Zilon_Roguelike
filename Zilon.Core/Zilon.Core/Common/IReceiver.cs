using System.Threading.Tasks;

namespace Zilon.Core.Common
{
    public interface IReceiver<T>
    {
        Task<T> ReceiveAsync();
    }
}