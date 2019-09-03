using System.Collections.Generic;

using ReGoap.Core;

using Zilon.Core.WorldGeneration.AgentMemories;

namespace Zilon.Core.WorldGeneration
{
    class BuilderAgent: ReGoapAgentAdvanced<string, object>
    {
        private readonly BuilderMemory _memory;

        public BuilderAgent(BuilderMemory memory)
        {
            _memory = memory;
        }

        public override void RefreshMemory()
        {
            base.RefreshMemory();
            memory = _memory;
        }

        public override void RefreshGoalsSet()
        {
            base.RefreshGoalsSet();

            goals = new List<IReGoapGoal<string, object>>(
                new IReGoapGoal<string, object>[] { new AgentGoals.BuildLocalityStructureGoal() }
                );
        }

        public override void RefreshActionsSet()
        {
            base.RefreshActionsSet();

            actions = new List<IReGoapAction<string, object>>(new IReGoapAction<string, object>[] {
                new AgentActions.CollectResourceAction(),
                new AgentActions.BuildLocalityStructureAction()
            });
        }
    }
}
