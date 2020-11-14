namespace Zilon.Core.Common
{
    public interface ISender<in T>
    {
        Task SendAsync(T obj);
    }
}