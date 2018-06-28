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
            foreach (var file in files)
            {
                var sid = Path.GetFileNameWithoutExtension(file);
                var serialized = File.ReadAllText(file);

                var schemeFile = new SchemeFile
                {
                    Sid = sid,
                    Content = serialized
                };

                result.Add(schemeFile);
            }

            return result.ToArray();
        }
    }
}
