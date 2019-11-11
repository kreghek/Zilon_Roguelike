using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Zilon.Core.Persons;

namespace Zilon.GlobeObserver
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            RegisterServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var personList = CreateGlobe(serviceProvider);

            while (true)
            {
                NextTurn(personList, serviceProvider);
            }
        }

        private static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IHumanPersonFactory, RandomHumanPersonFactory>();
        }

        private static void NextTurn(IList<IPerson> personList, IServiceProvider serviceProvider)
        {
            foreach (var person in personList)
            { 
                person.
            }
        }

        private static IList<IPerson> CreateGlobe(IServiceProvider serviceProvider)
        {
            var personList = new List<IPerson>();

            for (var localityIndex = 0; localityIndex < 300; localityIndex++)
            {
                var localitySector = CreateLocalitySector();

                for (var populationUnitIndex = 0; populationUnitIndex < 4; populationUnitIndex++)
                {
                    for (var personIndex = 0; personIndex < 10; personIndex++)
                    {
                        var person = CreatePerson(serviceProvider);
                        personList.Add(person);
                    }
                }
            }

            return personList;
        }

        private static IPerson CreatePerson(IServiceProvider serviceProvider)
        {
            var personFactory = serviceProvider.GetService<IHumanPersonFactory>();
            var person = personFactory.Create();
            return person;
        }

        private static object CreateLocalitySector()
        {
            throw new NotImplementedException();
        }
    }
}
