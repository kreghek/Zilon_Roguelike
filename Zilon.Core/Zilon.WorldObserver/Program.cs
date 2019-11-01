using System;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.WorldGeneration;
using Zilon.WorldObserver.Observers;

namespace Zilon.WorldObserver
{
    class Program
    {
        static async Task Main()
        {
            var dice = new LinearDice();
            var worldGenerator = new GlobeInitiator(dice);

            var globe = await worldGenerator.CreateStartGlobeAsync().ConfigureAwait(false);

            var observedObject = new LocalityObserver(globe.Localities.First());

            Console.WriteLine("World Created...");
            Console.ReadLine();

            var iterationCounter = 0;
            while (true)
            {
                Console.WriteLine($"[{iterationCounter} iteration]");

                await globe.NextIterationAsync();

                var textInfo = observedObject.WriteTextInfo();
                Console.WriteLine(textInfo);

                Console.ReadLine();

                iterationCounter++;
            }
        }
    }
}
