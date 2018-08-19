using Zilon.Core.Tactics;

namespace Zilon.Core.Persons
{
    public interface IActorStateEffect
    {
        void Apply(IActorState actorState);
    }
}
