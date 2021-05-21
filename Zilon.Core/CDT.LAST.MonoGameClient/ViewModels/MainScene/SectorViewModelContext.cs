using System.Collections.Generic;

using Zilon.Core.Tactics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class SectorViewModelContext
    {
        public SectorViewModelContext(ISector sector)
        {
            _sector = sector;

            GameObjects = new List<GameObjectBase>();
            EffectManager = new EffectManager();
        }

        public IEnumerable<IActor> GetActors()
        {
            return _sector.ActorManager.Items;
        }

        public EffectManager EffectManager { get; }

        public List<GameObjectBase> GameObjects { get; }

        private readonly ISector _sector;
    }
}