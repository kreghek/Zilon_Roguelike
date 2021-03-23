using System;

namespace Zilon.Core.Client.Sector
{
    public interface ICommandBlocker
    {
        void Release();
        event EventHandler Released;
    }
}