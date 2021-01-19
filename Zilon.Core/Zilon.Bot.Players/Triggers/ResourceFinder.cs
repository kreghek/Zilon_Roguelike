using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers
{
    public static class ResourceFinder
    {
        public static Resource FindBestConsumableResourceByRule(
            IActor actor,
            IActorTaskContext context,
            IEnumerable<Resource> resources,
            ConsumeCommonRuleType ruleType)
        {
            var foundResources = FindConsumableResourcesByRule(actor, context, resources, ruleType);

            var orderedResources = foundResources.OrderByDescending(x => x.Rule.Level);
            var bestResource = orderedResources.FirstOrDefault();

            return bestResource?.Resource;
        }

        public static IEnumerable<ResourceSelection> FindConsumableResourcesByRule(
            IActor actor,
            IActorTaskContext context,
            IEnumerable<Resource> resources,
            ConsumeCommonRuleType ruleType)
        {
            foreach (var resource in resources)
            {
                var rule = resource.Scheme.Use?.CommonRules?
                    .SingleOrDefault(x => x.Type == ruleType && x.Direction == PersonRuleDirection.Positive);

                if (rule != null)
                {
                    var isAllow = UsePropHelper.CheckPropAllowedByRestrictions(resource, actor, context);
                    if (!isAllow)
                    {
                        continue;
                    }

                    yield return new ResourceSelection
                    {
                        Resource = resource,
                        Rule = rule
                    };
                }
            }
        }
    }

    public class ResourceSelection
    {
        public Resource Resource { get; set; }
        public ConsumeCommonRule Rule { get; set; }
    }
}