using System.Collections.Generic;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class SectorViewModelContext
    {
        public SectorViewModelContext(EffectManager effectManager)
        {
            GameObjects = new List<GameObjectBase>();
            EffectManager = effectManager;
        }

        public EffectManager EffectManager { get; }

        public List<GameObjectBase> GameObjects { get; }
    }
}