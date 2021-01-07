using System;
using System.Threading.Tasks;

using Assets.Zilon.Scripts.Services;

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

    public sealed class TimeLimitedAnimationBlocker : AnimationCommonBlocker
    {
        public TimeLimitedAnimationBlocker()
        {
            Task.Delay(5000).ContinueWith(task =>
            {
                Release();
            });
        }
    }
}
