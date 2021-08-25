﻿using System;
using System.Linq;

using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class LootDetectedTrigger : ILogicStateTrigger
    {
        public void Reset()
        {
            // Нет состояния.
        }

        public bool Test(IActor actor, ISectorTaskSourceContext context, ILogicState currentState,
            ILogicStrategyData strategyData)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (currentState is null)
            {
                throw new ArgumentNullException(nameof(currentState));
            }

            if (strategyData is null)
            {
                throw new ArgumentNullException(nameof(strategyData));
            }

            var staticObjectManager = context.Sector.StaticObjectManager;
            var map = context.Sector.Map;

            var containers = staticObjectManager.Items.Where(x => x.HasModule<IPropContainer>());
            var foundContainers = LootHelper.FindAvailableContainers(containers,
                actor.Node,
                map);

            return foundContainers.Any();
        }

        public void Update()
        {
            // Нет состояния.
        }
    }
}