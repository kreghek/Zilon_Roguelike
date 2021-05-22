using System;

using Zilon.Core.Client.Sector;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class AnimationCommonBlocker : ICommandBlocker
    {
        public string? DebugName { get; set; }
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