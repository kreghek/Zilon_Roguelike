using System.Collections.Generic;

using Zilon.Core.Tactics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class SectorViewModelContext
    {
        private readonly ISector _sector;

        public SectorViewModelContext(ISector sector)
        {
            _sector = sector;

            GameObjects = new List<GameObjectBase>();
            EffectManager = new EffectManager();
        }

        public EffectManager EffectManager { get; }

        public List<GameObjectBase> GameObjects { get; }

        public ISector Sector => _sector;

        public IEnumerable<IActor> GetActors()
        {
            return Sector.ActorManager.Items;
        }
    }
}