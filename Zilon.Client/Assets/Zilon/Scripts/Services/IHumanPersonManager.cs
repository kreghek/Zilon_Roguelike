using Zilon.Core.Persons;
using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Services
{
    interface IHumanPersonManager
    {
        HumanPerson Person { get; set; }

        IActorState ActorState { get; set; }
    }
}
