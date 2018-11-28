using System.Linq;
using LightInject;

using TechTalk.SpecFlow;

using Zilon.Core.Spec.Contexts;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Spec.Steps
{
    [Binding]
    public sealed class PredefinedMapSteps : GenericStepsBase<CommonGameActionsContext>
    {
        public PredefinedMapSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [Given(@"Есть предопределённая карта (.+)")]
        public void GivenЕстьПредопределённаяКартаSid(string predefinedMapSid)
        {
            Context.CreateSector(5);
            var sector = Context.Container.GetInstance<ISector>();
            Context.AddMonsterActor("rat", 100, new OffsetCoords(0, 0));
            Context.AddMonsterActor("rat", 200, new OffsetCoords(4, 4));

            var patrolMonster = Context.GetMonsterById(100);
            sector.PatrolRoutes.Add(patrolMonster,
                new PatrolRoute(
                    sector.Map.Nodes.Cast<HexNode>().SelectBy(0, 0),
                    sector.Map.Nodes.Cast<HexNode>().SelectBy(4, 4)
                    ));
        }

    }
}
