using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using BenchmarkDotNet.Attributes;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Sdk;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.World;
using Zilon.Emulation.Common;

namespace Zilon.Core.Benchmarks.Move
{
    [MemoryDiagnoser]
    public class MassGamePlayBench
    {
        [Benchmark(Description = "Mass Game Play 40")]
        [SuppressMessage("Performance",
            "CA1822:Mark members as static",
            Justification = "Benchmarks MUST be instance methods, static methods are not supported.")]
        public async Task GamePlayBenchAsync()
        {
            var serviceContainer = new ServiceCollection();
            var startUp = new Startup();
            startUp.RegisterServices(serviceContainer);
            var serviceProvider = serviceContainer.BuildServiceProvider();

            var botSettings = new BotSettings { Mode = "duncan" };

            var globeInitializer = serviceProvider.GetRequiredService<IGlobeInitializer>();

            var autoPlayEngine = new AutoplayEngine(
                startUp,
                botSettings,
                globeInitializer);

            var globe = await autoPlayEngine.CreateGlobeAsync().ConfigureAwait(false);

            var context = new MassAutoplayContext(globe);

            await autoPlayEngine.StartAsync(globe, context).ConfigureAwait(false);
        }
    }

    internal sealed class MassAutoplayContext : IAutoplayContext
    {
        private readonly IGlobe _globe;
        private IPerson _currentFollowedPerson;

        public MassAutoplayContext(IGlobe globe)
        {
            UpdateFollowedPerson(globe);
            _globe = globe;
        }

        private void UpdateFollowedPerson(IGlobe globe)
        {
            _currentFollowedPerson = globe.SectorNodes.SelectMany(x => x.Sector.ActorManager.Items)
                .Where(x => x.Person.Fraction == Fractions.Pilgrims)
                .Where(x => !x.Person.GetModule<ISurvivalModule>().IsDead)
                .FirstOrDefault()?.Person;
        }

        public async Task<bool> CheckNextIterationAsync()
        {
            return await Task.Run(() =>
            {
                if (_currentFollowedPerson is null)
                {
                    UpdateFollowedPerson(_globe);
                }

                if (_currentFollowedPerson is null)
                {
                    return false;
                }

                return !_currentFollowedPerson.GetModule<ISurvivalModule>().IsDead;
            });
        }
    }
}