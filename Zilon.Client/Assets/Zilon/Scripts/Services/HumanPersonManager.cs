using Zilon.Core.Persons;
using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Services
{
    class HumanPersonManager : IHumanPersonManager
    {
        public HumanPerson Person { get; set; }

        public IActorState ActorState { get; set; }
    }
}
