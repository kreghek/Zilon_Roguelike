using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Zilon.Core.Schemes;

namespace Zilon.Core.CommonServices
{
    /// <summary>
    /// Класс для работы со схемами игрового мира.
    /// </summary>
    public sealed class SchemeService : ISchemeService
    {
        private readonly Dictionary<string, MapScheme> _maps;
        private readonly Dictionary<string, LocationScheme> _locations;
        private readonly Dictionary<string, PathScheme> _paths;

        public SchemeService(ISchemeLocator schemeLocator)
        {
            _maps = new Dictionary<string, MapScheme>();
            LoadSchemes(schemeLocator, "Maps", _maps);

            _locations = new Dictionary<string, LocationScheme>();
            LoadSchemes(schemeLocator, "Locations", _locations);

            _paths = new Dictionary<string, PathScheme>();
            LoadSchemes(schemeLocator, "Paths", _paths);
        }

        public TScheme GetScheme<TScheme>(string sid) where TScheme : class, IScheme
        {
            if (typeof(TScheme) == typeof(MapScheme))
            {
                return _maps[sid] as TScheme;
            }

            if (typeof(TScheme) == typeof(LocationScheme))
            {
                return _locations[sid] as TScheme;
            }

            if (typeof(TScheme) == typeof(PathScheme))
            {
                return _paths[sid] as TScheme;
            }

            throw new ArgumentException();
        }

        public IEnumerable<TScheme> GetSchemes<TScheme>() where TScheme : class, IScheme
        {
            if (typeof(TScheme) == typeof(MapScheme))
            {
                return _maps.Values.Cast<TScheme>();
            }

            if (typeof(TScheme) == typeof(LocationScheme))
            {
                return _locations.Values.Cast<TScheme>();
            }

            if (typeof(TScheme) == typeof(PathScheme))
            {
                return _paths.Values.Cast<TScheme>();
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