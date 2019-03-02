using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using Zilon.Core.Schemes;

namespace Zilon.SchemeEditor
{
    public class EditorSchemeServiceHandler<TSchemeImpl> : ISchemeServiceHandler<TSchemeImpl> where TSchemeImpl : class, IScheme
    {
        private const string SchemePostfix = "Scheme";

        private readonly Dictionary<string, TSchemeImpl> _dict;
        private readonly Dictionary<string, string> _dictPaths;
        private readonly ISchemeLocator _locator;
        private readonly string _directory;

        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        public EditorSchemeServiceHandler(ISchemeLocator locator)
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
            _dictPaths = new Dictionary<string, string>();
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
                    _dictPaths.Add(scheme.Sid, file.Path);
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException($"Ошибка при загрузке схемы {file}.", exception);
                }
            }
        }

        private TSchemeImpl ParseSchemeFromFile(SchemeFile file)
        {
            // Если явно указаны настройки десериализации, то используем их.
            if (JsonSerializerSettings == null)
            {
                return JsonConvert.DeserializeObject<TSchemeImpl>(file.Content);
            }

            return JsonConvert.DeserializeObject<TSchemeImpl>(file.Content, JsonSerializerSettings);
        }

        public TSchemeImpl Get(string sid)
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

        public void Update(TSchemeImpl scheme)
        {
            var sid = scheme.Sid;
            var path = _dictPaths[sid];
            var schemeContent = JsonConvert.SerializeObject(scheme, JsonSerializerSettings);
            File.WriteAllText(path, schemeContent);
        }

        private string CalcDirectory()
        {
            var type = typeof(TSchemeImpl);
            var typeName = type.Name;
            var schemeName = typeName.Substring(0, typeName.Length - SchemePostfix.Length);

            if (type.IsInterface)
            {
                schemeName = schemeName.Remove(0, 1);
            }

            var directory = schemeName + "s";
            return directory;
        }
    }
}
