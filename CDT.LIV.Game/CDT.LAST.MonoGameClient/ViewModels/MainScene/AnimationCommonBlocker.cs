using System;

using Zilon.Core.Client.Sector;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public sealed class AnimationCommonBlocker : ICommandBlocker
    {
        public event EventHandler? Released;

        public void Release()
        {
            DoRelease();
        }

        private void DoRelease()
        {
            Released?.Invoke(this, new EventArgs());
        }
    }
}
