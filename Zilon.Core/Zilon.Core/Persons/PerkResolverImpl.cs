using System;

using Zilon.Core.Tactics;

namespace Zilon.Core.Persons
{
    public class PerkResolverImpl : IPerkResolver
    {
        private readonly IActorManager _actorManager;

        public PerkResolverImpl(IActorManager actorManager)
        {
            _actorManager = actorManager;

            foreach (var actor in actorManager.Actors)
            {
                actor.OnDead += Actor_OnDead;
            }
        }

        private void Actor_OnDead(object sender, EventArgs e)
        {
            
        }

        public void CountDefeat(IActor hitActor, ITacticalAct act)
        {
            throw new NotImplementedException();
        }
    }
}
