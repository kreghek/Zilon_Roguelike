using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public class SchemeServiceHandler<TScheme> : ISchemeServiceHandler<TScheme> where TScheme : class, IScheme
    {
        private const string SCHEME_POSTFIX = "Scheme";

        private readonly Dictionary<string, TScheme> _dict;
        private readonly ISchemeLocator _locator;
        private readonly string _directory;

        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        public SchemeServiceHandler(ISchemeLocator locator)
        {
            _locator = locator;

            if (_directory == null)
            {
                _directory = CalcDirectory();
            }

            if (locator == null)
            {
                throw new ArgumentNullException(nameof(locator),
                    $"Ошибка при загрузки схем из директории {_directory}.");
            }

            _dict = new Dictionary<string, TScheme>();
        }

        public SchemeServiceHandler(ISchemeLocator locator, string directory) : this(locator)
        {
            _directory = directory;
        }

        public void LoadSchemes()
        {
            var files = _locator.GetAll(_directory);
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

                    TScheme scheme = ParseSchemeFromFile(file);

                    if (scheme.Disabled)
                    {
                        continue;
                    }

                    scheme.Sid = file.Sid;
                    _dict.Add(scheme.Sid, scheme);
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException($"Ошибка при загрузке схемы {file}.", exception);
                }
            }
        }

        private TScheme ParseSchemeFromFile(SchemeFile file)
        {
            // Если явно указаны настройки десеиализации, то используем их.
            if (JsonSerializerSettings == null)
            {
                return JsonConvert.DeserializeObject<TScheme>(file.Content);
            }

            return JsonConvert.DeserializeObject<TScheme>(file.Content, JsonSerializerSettings);
        }

        public TScheme Get(string sid)
        {
            try
            {
                return _dict[sid];
            }
            catch (KeyNotFoundException exception)
            {
                throw new InvalidOperationException($"Не найдена схема {_directory} Sid: {sid}.", exception);
            }
        }

        public TScheme[] GetAll()
        {
            return _dict.Values.Cast<TScheme>().ToArray();
        }

        private string CalcDirectory()
        {
            var type = typeof(TScheme);
            var typeName = type.Name;
            var schemeName = typeName.Substring(0, typeName.Length - SCHEME_POSTFIX.Length);

            if (type.IsInterface)
            {
                schemeName = schemeName.Remove(0, 1);
            }

            var directory = schemeName + "s";
            return directory;
        }
    }
}
