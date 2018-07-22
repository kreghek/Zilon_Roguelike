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
        private readonly Dictionary<string, PersonScheme> _persons;
        private readonly Dictionary<string, DropTableScheme> _trophyTables;

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

            _persons = new Dictionary<string, PersonScheme>();
            LoadSchemes(schemeLocator, "Persons", _persons);

//TODO Переименовать папку TrophyTables в DropTable
            _trophyTables = new Dictionary<string, DropTableScheme>();
            LoadSchemes(schemeLocator, "TrophyTables", _trophyTables);
        }

        public TScheme GetScheme<TScheme>(string sid) where TScheme : class, IScheme
        {
            //TODO Подумать, как избавиться от такой кучи if.
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

            if (typeof(TScheme) == typeof(PropScheme))
            {
                return _props[sid] as TScheme;
            }

            if (typeof(TScheme) == typeof(TacticalActScheme))
            {
                return _tacticalActs[sid] as TScheme;
            }

            if (typeof(TScheme) == typeof(PersonScheme))
            {
                return _persons[sid] as TScheme;
            }

            if (typeof(TScheme) == typeof(DropTableScheme))
            {
                return _trophyTables[sid] as TScheme;
            }

            throw new ArgumentException("Указан неизвестный тип схемы.");
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

            if (typeof(TScheme) == typeof(PropScheme))
            {
                return _props.Values.Cast<TScheme>().ToArray();
            }

            if (typeof(TScheme) == typeof(TacticalActScheme))
            {
                return _tacticalActs.Values.Cast<TScheme>().ToArray();
            }

            if (typeof(TScheme) == typeof(PersonScheme))
            {
                return _persons.Values.Cast<TScheme>().ToArray();
            }

            if (typeof(TScheme) == typeof(DropTableScheme))
            {
                return _trophyTables.Values.Cast<TScheme>().ToArray();
            }

            throw new ArgumentException("Указан неизвестный тип схемы.");
        }

        private void LoadSchemes<T>(ISchemeLocator schemeLocator,
            string directory,
            Dictionary<string, T> dict) where T : IScheme
        {
            if (schemeLocator == null)
            {
                throw new ArgumentNullException(nameof(schemeLocator),
                    $"Ошибка при загрузки схем из директории {directory}.");
            }

            var files = schemeLocator.GetAll(directory);
            foreach (var file in files)
            {
                try
                {
                    if (file == null)
                    {
                        throw new InvalidOperationException($"Файл схемы не может быть пустым.");
                    }

                    if (string.IsNullOrWhiteSpace(file.Content))
                    {
                        throw new InvalidOperationException($"Пустой контент схемы {file.Sid}.");
                    }

                    var scheme = JsonConvert.DeserializeObject<T>(file.Content);

                    if (scheme.Disabled)
                    {
                        continue;
                    }

                    scheme.Sid = file.Sid;
                    dict.Add(scheme.Sid, scheme);
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException($"Ошибка при загрузке схемы {file}.", exception);
                }
            }

        }
    }
}