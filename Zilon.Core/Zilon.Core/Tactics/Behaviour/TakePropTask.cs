using System;
using System.Collections.Generic;
using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Behaviour
{
    public class TakePropTask : ActorTaskBase
    {
        protected TakePropTask(IActor actor, IEnumerable<IProp> props) : base(actor)
        {
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
