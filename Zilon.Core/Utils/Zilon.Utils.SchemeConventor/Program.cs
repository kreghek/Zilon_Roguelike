using System.IO;

using Zilon.Core.Schemes;

namespace SchemeLocalizer
{
    class Program
    {
        static void Main(string[] args)
        {
            var binPath = Directory.GetCurrentDirectory();
            var schemeLocator = new FileSchemeLocator(binPath);
            var schemeFiles = schemeLocator.GetAll("props");

            foreach(var schemeFile in schemeFiles)
            {

            }
        }
    }
}
