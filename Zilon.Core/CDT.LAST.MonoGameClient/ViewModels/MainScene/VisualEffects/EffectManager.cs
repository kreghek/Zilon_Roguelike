using System.Collections.Generic;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.VisualEffects
{
    internal sealed class EffectManager
    {
        public EffectManager()
        {
            VisualEffects = new List<IVisualEffect>();
        }

        public List<IVisualEffect> VisualEffects { get; }
    }
}