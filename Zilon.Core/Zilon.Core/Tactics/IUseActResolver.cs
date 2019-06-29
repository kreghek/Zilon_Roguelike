using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    public interface IUseActResolver
    {
        bool SecondaryActUsePass(Actor actor, ITacticalAct act);
    }
}
