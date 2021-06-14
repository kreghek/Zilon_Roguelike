using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Zilon.Core.Schemes
{
    public class FileSchemeLocator : ISchemeLocator
    {
        private const string SCHEME_CATALOG_ENV_VARIABLE = "ZILON_LIV_SCHEME_CATALOG";
        private readonly string _schemeCatalog;

        [ExcludeFromCodeCoverage]
        public FileSchemeLocator(string schemeCatalog)
        {
            _schemeCatalog = schemeCatalog ?? throw new ArgumentNullException(nameof(schemeCatalog));

            var schemeLocatorFullPath = Path.GetFullPath(_schemeCatalog);

            if (!Directory.Exists(schemeLocatorFullPath))
            {
                throw new ArgumentException($"Директория каталога {schemeLocatorFullPath} не найдена.");
            }
        }

        [ExcludeFromCodeCoverage]
        public static FileSchemeLocator CreateFromEnvVariable()
        {
            var schemeCatalogFromEnvVariable = Environment.GetEnvironmentVariable(SCHEME_CATALOG_ENV_VARIABLE);
            if (string.IsNullOrWhiteSpace(schemeCatalogFromEnvVariable))
            {
                throw new InvalidOperationException($"Переменная окружения {SCHEME_CATALOG_ENV_VARIABLE} не задана.");
            }

            return new FileSchemeLocator(schemeCatalogFromEnvVariable);
        }

        private static string GetRelativePath(string path, string filePath, string sid)
        {
            var relativeFilePath = filePath.Remove(0, path.Length).TrimStart('\\');
            var fileFolder = relativeFilePath
                .Substring(0, relativeFilePath.Length - (sid + ".json").Length)
                .TrimEnd('\\');
            return fileFolder;
        }

        public SchemeFile[] GetAll(string directory)
        {
            var schemeLocatorFullPath = Path.GetFullPath(_schemeCatalog);

            var path = Path.Combine(schemeLocatorFullPath, directory);
            if (!Directory.Exists(path))
            {
                return Array.Empty<SchemeFile>();
            }

            var files = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
            var result = new List<SchemeFile>();
            foreach (var filePath in files)
            {
                var sid = Path.GetFileNameWithoutExtension(filePath);
                var serialized = File.ReadAllText(filePath);
                string fileFolder = GetRelativePath(path, filePath, sid);

                var schemeFile = new SchemeFile(serialized, fileFolder, sid);

                result.Add(schemeFile);
            }

            return result.ToArray();
        }
    }
}