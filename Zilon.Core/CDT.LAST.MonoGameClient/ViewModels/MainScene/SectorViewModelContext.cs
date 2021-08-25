using System.Collections.Generic;

using CDT.LAST.MonoGameClient.ViewModels.MainScene.VisualEffects;

using Zilon.Core.Tactics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal sealed class SectorViewModelContext
    {
        public SectorViewModelContext(ISector sector)
        {
            Sector = sector;

            GameObjects = new List<GameObjectBase>();
            EffectManager = new EffectManager();
            CorpseManager = new CorpseManager();
        }

        public CorpseManager CorpseManager { get; }

        public EffectManager EffectManager { get; }
        public List<GameObjectBase> GameObjects { get; }

        public ISector Sector { get; }

        public IEnumerable<IActor> GetActors()
        {
            return Sector.ActorManager.Items;
        }
    }
}