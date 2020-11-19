namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Файл схемы. Используется сервисов схем для внутренних нужд.
    /// </summary>
    public class SchemeFile
    {
        public string Content { get; set; }
        public string Path { get; set; }
        public string Sid { get; set; }

        public override string ToString()
        {
            return $"{Sid} (Size: {Content?.Length}) in \"{Path}\"";
        }
    }
}