using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Bot.Players.Triggers
{
    public static class ResourceFinder
    {
        public static Resource FindBestConsumableResourceByRule(
            IEnumerable<Resource> resources,
            ConsumeCommonRuleType ruleType)
        {
            var foundResources = FindConsumableResourcesByRule(resources, ruleType);

            var orderedResources = foundResources.OrderByDescending(x => x.Rule.Level);
            var bestResource = orderedResources.FirstOrDefault();

            return bestResource?.Resource;
        }

        public static IEnumerable<ResourceSelection> FindConsumableResourcesByRule(
            IEnumerable<Resource> resources,
            ConsumeCommonRuleType ruleType)
        {
            foreach (var resource in resources)
            {
                var rule = resource.Scheme.Use?.CommonRules?
                    .SingleOrDefault(x => (x.Type == ruleType) && (x.Direction == PersonRuleDirection.Positive));

                if (rule != null)
                {
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