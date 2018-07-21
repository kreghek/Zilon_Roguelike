using AutoMapper;
using Newtonsoft.Json;
using System.IO;
using Zilon.Core.Utils.SourceSchemes;

namespace SchemeLocalizer
{
    class Program
    {
        static void Main(string[] args)
        {
            var binPath = Directory.GetCurrentDirectory();

            var targetPath = Path.Combine(binPath, "result", "props");
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            var schemeLocator = new Zilon.Core.Schemes.FileSchemeLocator(binPath);
            var schemeFiles = schemeLocator.GetAll("props");

            Mapper.Initialize(cfg => cfg
            .CreateMap<PropScheme, Zilon.Core.Schemes.PropScheme>()
            .ForMember(x => x.Name, opt1 => opt1.Ignore())
            .ForMember(x => x.Description, opt1 => opt1.Ignore())
            );

            foreach (var schemeFile in schemeFiles)
            {
                var scheme = JsonConvert.DeserializeObject<PropScheme>(schemeFile.Content);

                var targetScheme = new Zilon.Core.Schemes.PropScheme
                {
                    Name = new Zilon.Core.Schemes.LocalizedStringSubScheme
                    {
                        Ru = scheme.Name
                    }
                };

                Mapper.Map(scheme, targetScheme);

                var targetSerialized = JsonConvert.SerializeObject(targetScheme, 
                    Formatting.Indented,
                    new JsonSerializerSettings {
                        Formatting = Formatting.Indented,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });
                var targetFilePath = Path.Combine(targetPath);
                if (schemeFile.Path != "\\")
                {
                    targetFilePath = Path.Combine(targetFilePath);
                }
                
                targetFilePath = Path.Combine(targetFilePath, schemeFile.Sid + ".json");
                using (var writer = File.CreateText(targetFilePath))
                {
                    writer.Write(targetSerialized);
                }
            }
        }
    }
}
