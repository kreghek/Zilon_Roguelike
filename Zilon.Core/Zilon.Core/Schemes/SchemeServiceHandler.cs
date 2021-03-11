using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public class SchemeServiceHandler<TSchemeImpl> : ISchemeServiceHandler<TSchemeImpl>
        where TSchemeImpl : class, IScheme
    {
        private const string SCHEME_POSTFIX = "Scheme";

        private readonly Dictionary<string, TSchemeImpl> _dict;
        private readonly string _directory;
        private readonly ISchemeLocator _locator;

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

            _dict = new Dictionary<string, TSchemeImpl>();
        }

        public JsonSerializerSettings? JsonSerializerSettings { get; set; }

        private static string CalcDirectory()
        {
            var type = typeof(TSchemeImpl);
            var typeName = type.Name;
            var schemeName = typeName.Substring(0, typeName.Length - SCHEME_POSTFIX.Length);

            if (type.IsInterface)
            {
                schemeName = schemeName.Remove(0, 1);
            }

            var directory = schemeName + "s";
            return directory;
        }

        private TSchemeImpl ParseSchemeFromFile(SchemeFile file)
        {
            // Если явно указаны настройки десериализации, то используем их.
            if (JsonSerializerSettings is null)
            {
                return JsonConvert.DeserializeObject<TSchemeImpl>(file.Content);
            }

#pragma warning disable CS8603 // Possible null reference return.
            return JsonConvert.DeserializeObject<TSchemeImpl>(file.Content, JsonSerializerSettings);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public void LoadSchemes()
        {
            var files = _locator.GetAll(_directory);
            foreach (var file in files)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(file.Content))
                    {
                        throw new InvalidOperationException($"Пустой контент схемы {file.Sid}.");
                    }

                    var scheme = ParseSchemeFromFile(file);

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

        public TSchemeImpl GetItem(string sid)
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

        public TSchemeImpl[] GetAll()
        {
            return _dict.Values.ToArray();
        }
    }
}