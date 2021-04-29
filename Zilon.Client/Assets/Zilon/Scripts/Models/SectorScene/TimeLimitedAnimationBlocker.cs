using System.Threading.Tasks;

namespace Assets.Zilon.Scripts.Models.SectorScene
{
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
