using System;

namespace Assets.Zilon.Scripts.Services
{
    public interface ICommandBlocker
    {
        event EventHandler Released;
        void Release();
    }
}
