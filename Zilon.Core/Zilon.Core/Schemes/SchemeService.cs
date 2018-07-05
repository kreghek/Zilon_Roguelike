using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Класс для работы со схемами игрового мира.
    /// </summary>
    public sealed class SchemeService : ISchemeService
    {
        private readonly Dictionary<string, MapScheme> _maps;
        private readonly Dictionary<string, LocationScheme> _locations;
        private readonly Dictionary<string, PathScheme> _paths;
        private readonly Dictionary<string, PropScheme> _props;
        private readonly Dictionary<string, TacticalActScheme> _tacticalActs;

        public SchemeService(ISchemeLocator schemeLocator)
        {
            _maps = new Dictionary<string, MapScheme>();
            LoadSchemes(schemeLocator, "Maps", _maps);

            _locations = new Dictionary<string, LocationScheme>();
            LoadSchemes(schemeLocator, "Locations", _locations);

            _paths = new Dictionary<string, PathScheme>();
            LoadSchemes(schemeLocator, "Paths", _paths);

            _props = new Dictionary<string, PropScheme>();
            LoadSchemes(schemeLocator, "Props", _props);

            _tacticalActs = new Dictionary<string, TacticalActScheme>();
            LoadSchemes(schemeLocator, "TacticalActs", _tacticalActs);
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

        public TScheme[] GetSchemes<TScheme>() where TScheme : class, IScheme
        {
            if (typeof(TScheme) == typeof(MapScheme))
            {
                return _maps.Values.Cast<TScheme>().ToArray();
            }

            if (typeof(TScheme) == typeof(LocationScheme))
            {
                return _locations.Values.Cast<TScheme>().ToArray();
            }

            if (typeof(TScheme) == typeof(PathScheme))
            {
                return _paths.Values.Cast<TScheme>().ToArray();
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
                {
                    continue;
                }

                scheme.Sid = file.Sid;
                dict.Add(scheme.Sid, scheme);
            }
        }
    }
}