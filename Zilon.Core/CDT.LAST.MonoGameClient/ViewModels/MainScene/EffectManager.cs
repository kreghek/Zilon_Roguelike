using System.Collections.Generic;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public sealed class EffectManager
    {
        public EffectManager()
        {
            HitEffects = new List<HitEffect>();
        }

        public List<HitEffect> HitEffects { get; }
    }
}