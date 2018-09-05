using System.Linq;

using FluentAssertions;

using TechTalk.SpecFlow;

using Zilon.Core.Spec.Contexts;

namespace Zilon.Core.Spec.Steps
{
    [Binding]
    public sealed class PerkSteps : GenericStepsBase<CommonGameActionsContext>
    {
        public PerkSteps(CommonGameActionsContext context) : base(context)
        {

        }

        [Given(@"У актёра игрока прогресс перка на убийство (.*) из (.*)")]
        public void GivenУАктёраИгрокаПрогрессПеркаНаУбийствоИз(int perkProgress, int perkGoal)
        {
            const string perkSid = "battle-dogmas";

            var actor = _context.GetActiveActor();

            var perk = actor.Person.EvolutionData.Perks.Single(x => x.Scheme.Sid == perkSid);

            perk.CurrentJobs[0].Progress = perkProgress;
        }

        [Then(@"перк на убийство должен быть прокачен")]
        public void ThenПеркНаУбийствоДолженБытьПрокачен()
        {
            var actor = _context.GetActiveActor();

            var perk = actor.Person.EvolutionData.Perks[0];

            perk.CurrentLevel.Should().NotBeNull("Перк должен быть прокачен. null в уровне означает непрокаченный перк.");
            perk.CurrentLevel.Primary.Should().Be(0);
            perk.CurrentLevel.Sub.Should().Be(0);
        }
    }
}