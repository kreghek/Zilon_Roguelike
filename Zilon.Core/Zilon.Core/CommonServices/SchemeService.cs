namespace Zilon.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    using Zilon.Core.Schemes;

    /// <summary>
    /// Класс для работы со схемами игрового мира.
    /// </summary>
    public sealed class SchemeService : ISchemeService
    {
        private readonly Dictionary<string, MapScheme> maps;
        private readonly Dictionary<string, LocationScheme> locations;
        private readonly Dictionary<string, PathScheme> paths;

        public SchemeService(ISchemeLocator schemeLocator)
        {
            maps = new Dictionary<string, MapScheme>();
            LoadSchemes(schemeLocator, "Maps", maps);

            locations = new Dictionary<string, LocationScheme>();
            LoadSchemes(schemeLocator, "Locations", locations);

            paths = new Dictionary<string, PathScheme>();
            LoadSchemes(schemeLocator, "Paths", paths);
        }

        public TScheme GetScheme<TScheme>(string sid) where TScheme : class, IScheme
        {
            if (typeof(TScheme) == typeof(MapScheme))
            {
                return maps[sid] as TScheme;
            }

            if (typeof(TScheme) == typeof(LocationScheme))
            {
                return locations[sid] as TScheme;
            }

            if (typeof(TScheme) == typeof(PathScheme))
            {
                return paths[sid] as TScheme;
            }

            throw new ArgumentException();
        }

        public IEnumerable<TScheme> GetSchemes<TScheme>() where TScheme : class, IScheme
        {
            if (typeof(TScheme) == typeof(MapScheme))
            {
                return maps.Values.Cast<TScheme>();
            }

            if (typeof(TScheme) == typeof(LocationScheme))
            {
                return locations.Values.Cast<TScheme>();
            }

            if (typeof(TScheme) == typeof(PathScheme))
            {
                return paths.Values.Cast<TScheme>();
            }

            throw new ArgumentException();
        }

        private void LoadSchemes<T>(ISchemeLocator schemeLocator, string directory, Dictionary<string, T> dict) where T : IScheme
        {
            var files = schemeLocator.GetAll(directory);
            foreach (var file in files)
            {
                var scheme = JsonConvert.DeserializeObject<T>(file.Content);

                if (scheme.Disabled)
                    continue;

                scheme.Sid = file.Sid;
                dict.Add(scheme.Sid, scheme);
            }
        }
    }
}