using System;

namespace Zilon.Core.Client.Sector
{
    public interface ICommandBlocker
    {
        event EventHandler Released;
        void Release();
    }
}
