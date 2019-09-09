using System.Collections.Generic;
using System.Linq;
using ReGoap.Core;
using Zilon.Core.WorldGeneration.AgentActions;
using Zilon.Core.WorldGeneration.AgentGoals;
using Zilon.Core.WorldGeneration.AgentMemories;

namespace Zilon.Core.WorldGeneration
{
    class BuilderAgent: ReGoapAgentAdvanced<string, object>
    {
        private readonly BuilderMemory _memory;
        private readonly Globe _globe;
        private readonly Agent _agent;

        public BuilderAgent(BuilderMemory memory, Globe globe, Agent agent)
        {
            _memory = memory;
            _globe = globe;
            _agent = agent;
        }

        public override void RefreshMemory()
        {
            base.RefreshMemory();
            memory = _memory;
        }

        public override void RefreshGoalsSet()
        {
            base.RefreshGoalsSet();

            var agentRealm = _agent.Realm;
            var realmLocalities = _globe.Localities.Where(x => x.Owner == agentRealm).ToArray();

            goals = new List<IReGoapGoal<string, object>>();

            foreach (var locality in realmLocalities)
            {
                foreach (var structure in LocalityStructureRepository.All)
                {
                    goals.Add(new BuildLocalityStructureGoal(locality, structure));

                    // Пока просто выбираем целью строительство первого попавшегося здания.
                    return;
                }
            }
        }

        public override void RefreshActionsSet()
        {
            base.RefreshActionsSet();

            actions = new List<IReGoapAction<string, object>>(new IReGoapAction<string, object>[] {
                //new CollectResourceAction() { Name = "Collect Resources" }
            });

            var agentRealm = _agent.Realm;
            var realmLocalities = _globe.Localities.Where(x => x.Owner == agentRealm).ToArray();

            foreach (var locality in realmLocalities)
            {
                var structure = LocalityStructureRepository.GarmentFactory;
                actions.Add(new BuildLocalityStructureAction(locality, structure) { Name = $"Build {structure.Name} in {locality}" });

                //foreach (var structure in LocalityStructureRepository.All)
                //{
                //    actions.Add(new BuildLocalityStructureAction(locality, structure){ Name = $"Build {structure.Name} in {locality}" });
                //}
            }
        }
    }
}
