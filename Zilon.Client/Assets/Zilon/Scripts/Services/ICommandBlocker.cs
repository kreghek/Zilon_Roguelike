using System;

namespace Assets.Zilon.Scripts.Services
{
    interface ICommandBlocker
    {
        event EventHandler Released;
        void Release();
    }
}
