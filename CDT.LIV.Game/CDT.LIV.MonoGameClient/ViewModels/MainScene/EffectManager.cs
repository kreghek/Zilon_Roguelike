using System.Collections.Generic;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public sealed class EffectManager
    {
        public List<HitEffect> HitEffects { get; }

        public EffectManager()
        {
            HitEffects = new List<HitEffect>();
        }
    }
}
