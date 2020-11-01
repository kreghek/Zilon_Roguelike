using System.Linq;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.Tactics;

namespace Zilon.Core.Specs.Steps
{
    [Binding]
    public sealed class PerkSteps : GenericStepsBase<CommonGameActionsContext>
    {
        public PerkSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [Given(@"У актёра игрока прогресс (\d+) перка (.+)")]
        public void GivenУАктёраИгрокаПрогрессПеркаНаУбийствоИз(int perkProgress, string perkSid)
        {
            IActor actor = Context.GetActiveActor();

            IPerk perk = actor.Person.GetModule<IEvolutionModule>().Perks.Single(x => x.Scheme.Sid == perkSid);

            perk.CurrentJobs[0].Progress = perkProgress;
        }

        [Then(@"Перк (.+) должен быть прокачен")]
        public void ThenПеркДолженБытьПрокачен(string perkSid)
        {
            IActor actor = Context.GetActiveActor();

            IPerk perk = actor.Person.GetModule<IEvolutionModule>().Perks.Single(x => x.Scheme.Sid == perkSid);

            perk.CurrentLevel.Should()
                .NotBeNull("Перк должен быть прокачен. null в уровне означает непрокаченный перк.");
            perk.CurrentLevel.Primary.Should().Be(0);
            perk.CurrentLevel.Sub.Should().Be(0);
        }
    }
}