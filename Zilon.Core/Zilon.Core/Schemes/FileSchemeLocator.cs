using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using JetBrains.Annotations;

namespace Zilon.Core.Schemes
{
    public class FileSchemeLocator : ISchemeLocator
    {
        private readonly string _schemeCatalog;

        [ExcludeFromCodeCoverage]
        public FileSchemeLocator([NotNull] string schemeCatalog)
        {
            _schemeCatalog = schemeCatalog ?? throw new System.ArgumentNullException(nameof(schemeCatalog));

            if (!Directory.Exists(_schemeCatalog))
            {
                throw new System.ArgumentException($"Директория каталога {_schemeCatalog} не найдена.");
            }
        }

        public SchemeFile[] GetAll(string directory)
        {
            var path = Path.Combine(_schemeCatalog, directory);
            if (!Directory.Exists(path))
            {
                return System.Array.Empty<SchemeFile>();
            }

            var files = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
            var result = new List<SchemeFile>();
            foreach (var filePath in files)
            {
                var sid = Path.GetFileNameWithoutExtension(filePath);
                var serialized = File.ReadAllText(filePath);
                string fileFolder = GetRelativePath(path, filePath, sid);

                var schemeFile = new SchemeFile
                {
                    Sid = sid,
                    Path = fileFolder,
                    Content = serialized
                };

                result.Add(schemeFile);
            }

            return result.ToArray();
        }

        private static string GetRelativePath(string path, string filePath, string sid)
        {
            var relativeFilePath = filePath.Remove(0, path.Length).TrimStart('\\');
            var fileFolder = relativeFilePath
                .Substring(0, relativeFilePath.Length - (sid + ".json").Length)
                .TrimEnd('\\');
            return fileFolder;
        }
    }
}
