using System;

using Zilon.Core.Client.Sector;

namespace Assets.Zilon.Scripts.Models.SectorScene
{
    public class AnimationCommonBlocker : ICommandBlocker
    {
        public event EventHandler Released;

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
