using Zilon.Core.World;

namespace Assets.Zilon.Scripts.Services
{
    class GlobeStorage
    {
        public IGlobe Globe { get; private set; }

        public void AssignGlobe(IGlobe globe)
        {
            Globe = globe;
        }
    }
}
