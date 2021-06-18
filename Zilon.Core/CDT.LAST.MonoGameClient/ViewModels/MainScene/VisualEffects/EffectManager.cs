using System.Collections.Generic;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.VisualEffects
{
    public sealed class EffectManager
    {
        public EffectManager()
        {
            VisualEffects = new List<IVisualEffect>();
        }

        public List<IVisualEffect> VisualEffects { get; }
    }
}