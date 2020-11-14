using System.Linq;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using TechTalk.SpecFlow;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Specs.Contexts;

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
            var actor = Context.GetActiveActor();

            var perk = actor.Person.GetModule<IEvolutionModule>().Perks.Single(x => x.Scheme.Sid == perkSid);

            perk.CurrentJobs[0].Progress = perkProgress;
        }

        [Given(@"Актёр игрока получает перк (.+)")]
        public void GivenАктёрИгрокаПолучаетПерк(string perkSid)
        {
            var schemeService = Context.ServiceProvider.GetRequiredService<ISchemeService>();

            var perkScheme = schemeService.GetScheme<IPerkScheme>(perkSid);
            var perk = new Perk
            {
                Scheme = perkScheme
            };

            var actor = Context.GetActiveActor();

            actor.Person.GetModule<IEvolutionModule>().AddBuildInPerks(new[] { perk });
        }

        [Then(@"Перк (.+) должен быть прокачен")]
        public void ThenПеркДолженБытьПрокачен(string perkSid)
        {
            var actor = Context.GetActiveActor();

            var perk = actor.Person.GetModule<IEvolutionModule>().Perks.Single(x => x.Scheme.Sid == perkSid);

            perk.CurrentLevel.Should().NotBeNull("Перк должен быть прокачен. null в уровне означает непрокаченный перк.");
            perk.CurrentLevel.Primary.Should().Be(0);
            perk.CurrentLevel.Sub.Should().Be(0);
        }
    }
}