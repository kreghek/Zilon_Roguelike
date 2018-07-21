using AutoMapper;
using Newtonsoft.Json;
using System.IO;
using OldPropScheme = Silone.Data.Schemes.PropScheme;
using CurrentPropScheme = Zilon.Core.Schemes.PropScheme;

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

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<OldPropScheme, CurrentPropScheme>()
                .ForMember(x => x.Name, opt1 => opt1.Ignore())
                .ForMember(x => x.Description, opt1 => opt1.Ignore());

                cfg.CreateMap<Silone.Data.Schemes.PropSet, Zilon.Core.Schemes.PropSet>()
                .ForMember(x => x.Count, opt1 => opt1.MapFrom(s => s.MinCount));
            }

            
            );

            foreach (var schemeFile in schemeFiles)
            {
                var oldScheme = JsonConvert.DeserializeObject<OldPropScheme>(schemeFile.Content);

                var targetScheme = new CurrentPropScheme
                {
                    Name = new Zilon.Core.Schemes.LocalizedStringSubScheme
                    {
                        En = oldScheme.Name
                    }
                };

                if (oldScheme.EquipmentSlot != Silone.Data.Schemes.EquipmentSlot.None)
                {
                    targetScheme.Equip = new Zilon.Core.Schemes.PropEquipSubScheme
                    {
                        Absorbtion = oldScheme.Absorbtion,
                        ActSids = new[] { "chop" },
                        ApRank = oldScheme.ArmorPenetration,
                        ArmorRank = oldScheme.Armor,
                        Power = oldScheme.Power
                    };
                }

                Mapper.Map(oldScheme, targetScheme);

                var targetSerialized = JsonConvert.SerializeObject(targetScheme, 
                    Formatting.Indented,
                    new JsonSerializerSettings {
                        Formatting = Formatting.Indented,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    });
                var targetFilePath = Path.Combine(targetPath);
                if (schemeFile.Path != "\\")
                {
                    targetFilePath = Path.Combine(targetFilePath, schemeFile.Path);
                }

                if (!Directory.Exists(targetFilePath))
                {
                    Directory.CreateDirectory(targetFilePath);
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
