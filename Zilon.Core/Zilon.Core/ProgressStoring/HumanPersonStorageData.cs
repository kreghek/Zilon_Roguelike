using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.ProgressStoring
{
    public class HumanPersonStorageData
    {
        public HumanSurvivalStatStorageData[] Survival { get; set; }
        public PropStorageData[] Equipments { get; set; }

        public static HumanPersonStorageData Create(HumanPerson humanPerson)
        {
            var storageData = new HumanPersonStorageData();

            storageData.Survival = humanPerson.Survival.Stats.Select(x => new HumanSurvivalStatStorageData
            {
                Type = x.Type,
                Value = x.Value
            }).ToArray();

            storageData.Equipments = humanPerson.EquipmentCarrier.Select(x => x == null ? null : new PropStorageData
            {
                Sid = x.Scheme.Sid,
                Durable = x.Durable.Value
            }).ToArray();

            return storageData;
        }

        public HumanPerson Restore(ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPropFactory propFactory)
        {
            var storedPerson = this;

            var personScheme = schemeService.GetScheme<IPersonScheme>("human-person");

            var inventory = new Inventory();

            var evolutionData = new EvolutionData(schemeService);

            var defaultActScheme = schemeService.GetScheme<ITacticalActScheme>(personScheme.DefaultAct);

            var person = new HumanPerson(personScheme,
                                         defaultActScheme,
                                         evolutionData,
                                         survivalRandomSource,
                                         inventory);

            foreach (var survivalStoredItem in storedPerson.Survival)
            {
                var stat = person.Survival.Stats.Single(x=>x.Type == survivalStoredItem.Type);
                stat.Value = survivalStoredItem.Value;
            }

            for (var i = 0; i < storedPerson.Equipments.Length; i++)
            {
                var storedEquipment = storedPerson.Equipments[i];

                if (storedEquipment == null)
                {
                    continue;
                }

                var equipmentScheme = schemeService.GetScheme<IPropScheme>(storedEquipment.Sid);

                var equipment = propFactory.CreateEquipment(equipmentScheme);
                equipment.Durable.Value = storedEquipment.Durable;

                person.EquipmentCarrier[i] = equipment;
                //TODO Уменьшать прочность согласно сохранённым данным
            }

            return person;
        }
    }
}
