using System;

using Assets.Zilon.Scripts.Services;

namespace Assets.Zilon.Scripts.Models.SectorScene
{
    public class SleepBlocker : ICommandBlocker
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
