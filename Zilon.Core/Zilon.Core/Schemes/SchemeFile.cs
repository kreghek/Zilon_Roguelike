using System;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Файл схемы. Используется сервисов схем для внутренних нужд.
    /// </summary>
    public class SchemeFile
    {
        public SchemeFile(string content, string path, string sid)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Sid = sid ?? throw new ArgumentNullException(nameof(sid));
        }

        public string Content { get; set; }
        public string Path { get; set; }
        public string Sid { get; set; }

        public override string ToString()
        {
            return $"{Sid} (Size: {Content?.Length}) in \"{Path}\"";
        }
    }
}