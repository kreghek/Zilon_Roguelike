using System;

using Zilon.Core.Client.Sector;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public sealed class AnimationCommonBlocker : ICommandBlocker
    {
        private void DoRelease()
        {
            Released?.Invoke(this, new EventArgs());
        }

        public event EventHandler? Released;

        public void Release()
        {
            DoRelease();
        }
    }
}