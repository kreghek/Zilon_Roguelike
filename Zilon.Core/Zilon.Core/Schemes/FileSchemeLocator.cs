using System.Collections.Generic;
using System.IO;

namespace Zilon.Core.Schemes
{
    public class FileSchemeLocator : ISchemeLocator
    {
        private readonly string _schemeCatalog;

        public FileSchemeLocator(string schemeCatalog)
        {
            _schemeCatalog = schemeCatalog;
        }

        public SchemeFile[] GetAll(string directory)
        {
            var path = Path.Combine(_schemeCatalog, directory);
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
            var relativeFilePath = filePath.Remove(0, path.Length);
            var fileFolder = relativeFilePath.Substring(0, relativeFilePath.Length - (sid + ".json").Length);
            return fileFolder;
        }
    }
}
