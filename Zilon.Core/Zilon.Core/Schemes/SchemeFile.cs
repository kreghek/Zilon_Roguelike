using System;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Файл схемы. Используется сервисов схем для внутренних нужд.
    /// </summary>
    public class SchemeFile
    {
        public SchemeFile(string content, string? path, string sid)
        {
            Content = content;
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Sid = sid;
        }

        public SchemeFile(string content, string sid)
        {
            Content = content;
            Sid = sid;
        }

        public string Content { get; set; }
        public string? Path { get; set; }
        public string Sid { get; set; }

        public override string ToString()
        {
            if (Path is null)
            {
                return $"{Sid} (Size: {Content.Length})";
            }

            return $"{Sid} (Size: {Content.Length}) in \"{Path}\"";
        }
    }
}