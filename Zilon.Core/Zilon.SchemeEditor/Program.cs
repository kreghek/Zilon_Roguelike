using System;
using System.Collections.Generic;
using System.Configuration;

using Zilon.Core.Schemes;

namespace Zilon.SchemeEditor
{
    class Program
    {
        private static ISchemeService _schemeService;

        static void Main(string[] args)
        {
            _schemeService = CreateSchemeService();

            Console.WriteLine("[1] Monsters");
            Console.WriteLine();

            while (true)
            {
                var command = Console.ReadKey();
                if (command.KeyChar == '1')
                {
                    var schemeDict = ListMonsterSchemes();
                    var choose = Console.ReadLine();

                    var scheme = schemeDict[int.Parse(choose)];

                    EditMonsterScheme(scheme);
                }
            }
        }

        private static void EditMonsterScheme(IScheme scheme)
        {
            Console.WriteLine($"Name Ru: {scheme.Name?.Ru}");
            var nameRu = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nameRu))
            {

            }
            else
            {
                Console.Write("[Old value]");
                Console.WriteLine();
            }
        }

        private static Dictionary<int, IScheme> ListMonsterSchemes()
        {
            var dict = new Dictionary<int, IScheme>();
            var monsterSchemes = _schemeService.GetSchemes<IMonsterScheme>();
            var i = 1;
            foreach (var monsterScheme in monsterSchemes)
            {
                Console.WriteLine($"[{i}] {monsterScheme}");
                dict.Add(i, monsterScheme);
                i++;
            }

            return dict;
        }

        private static ISchemeService CreateSchemeService()
        {
            var schemePath = Environment.GetEnvironmentVariable("ZILON_LIV_SCHEME_CATALOG");

            var schemeLocator = new FileSchemeLocator(schemePath);

            var schemeHandlerFactory = new EditorSchemeServiceHandlerFactory(schemeLocator);

            return new SchemeService(schemeHandlerFactory);
        }
    }
}
