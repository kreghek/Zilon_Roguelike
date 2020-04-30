using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.ProgressStoring
{
    public class HumanPersonStorageData
    {
        public HumanSurvivalStatStorageData[] Survival { get; set; }
        public PropStorageData[] Equipments { get; set; }
        public PropStorageData[] Inventory { get; set; }
        public PerkStorageData[] Perks { get; set; }

        public static HumanPersonStorageData Create(HumanPerson humanPerson)
        {
            if (humanPerson is null)
            {
                throw new ArgumentNullException(nameof(humanPerson));
            }

            var storageData = new HumanPersonStorageData
            {
                Survival = humanPerson.GetModule<ISurvivalModule>().Stats.Select(x => new HumanSurvivalStatStorageData
                {
                    Type = x.Type,
                    Value = x.ValueShare
                }).ToArray(),

                Equipments = humanPerson.GetModule<IEquipmentModule>().Select(CreateEquipmentStorageData).ToArray(),

                Inventory = humanPerson.GetModule<IInventoryModule>().CalcActualItems().Select(CreatePropStorageData).ToArray(),

                Perks = humanPerson.GetModule<IEvolutionModule>().Perks.Select(CreatePerkStorageData).ToArray()
            };

            return storageData;
        }

        private static PerkStorageData CreatePerkStorageData(IPerk x)
        {
            return new PerkStorageData
            {
                Sid = x.Scheme.Sid,
                Level = x.CurrentLevel?.Primary,
                SubLevel = x.CurrentLevel?.Sub,
                Jobs = x.CurrentJobs.Select(job => new PerkJobStorageData
                {
                    Type = job.Scheme.Type,
                    Scope = job.Scheme.Scope,
                    Progress = job.Progress,
                    IsComplete = job.IsComplete
                }).ToArray()
            };
        }

        private static PropStorageData CreateEquipmentStorageData(IProp prop)
        {
            if (prop == null)
            {
                return null;
            }

            return CreatePropStorageData(prop);
        }

        private static PropStorageData CreatePropStorageData(IProp prop)
        {
            var storageData = new PropStorageData
            {
                Sid = prop.Scheme.Sid
            };

            switch (prop)
            {
                case Equipment equipment:
                    storageData.Type = PropType.Equipment;
                    storageData.Durable = equipment.Durable.Value;
                    break;

                case Resource resource:
                    storageData.Type = PropType.Resource;
                    storageData.Count = resource.Count;
                    break;
            }

            return storageData;
        }

        public HumanPerson Restore(ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPropFactory propFactory)
        {
            if (schemeService is null)
            {
                throw new ArgumentNullException(nameof(schemeService));
            }

            if (survivalRandomSource is null)
            {
                throw new ArgumentNullException(nameof(survivalRandomSource));
            }

            if (propFactory is null)
            {
                throw new ArgumentNullException(nameof(propFactory));
            }

            var storedPerson = this;

            var personScheme = schemeService.GetScheme<IPersonScheme>("human-person");

            var inventory = new InventoryModule();

            var evolutionData = new EvolutionModule(schemeService);

            RestoreEvolutionData(schemeService, storedPerson, evolutionData);

            var defaultActScheme = schemeService.GetScheme<ITacticalActScheme>(personScheme.DefaultAct);

            var person = new HumanPerson(personScheme,
                                         defaultActScheme,
                                         evolutionData,
                                         survivalRandomSource,
                                         inventory);

            foreach (var survivalStoredItem in storedPerson.Survival)
            {
                var normalizedValueShare = RangeHelper.NormalizeShare(survivalStoredItem.Value);

                var stat = person.GetModule<ISurvivalModule>().Stats.Single(x => x.Type == survivalStoredItem.Type);

                stat.SetShare(normalizedValueShare);
            }

            foreach (var storedProp in storedPerson.Inventory)
            {
                var propScheme = schemeService.GetScheme<IPropScheme>(storedProp.Sid);
                IProp prop;
                switch (storedProp.Type)
                {
                    case PropType.Resource:
                        prop = propFactory.CreateResource(propScheme, storedProp.Count);
                        break;

                    case PropType.Equipment:
                        var equipment = propFactory.CreateEquipment(propScheme);
                        equipment.Durable.Value = storedProp.Durable;
                        prop = equipment;

                        break;

                    default:
                        throw new Exception();
                }

                inventory.Add(prop);
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

                person.GetModule<IEquipmentModule>()[i] = equipment;
                //TODO Уменьшать прочность согласно сохранённым данным
            }

            return person;
        }

        private static void RestoreEvolutionData(ISchemeService schemeService,
            HumanPersonStorageData storedPerson,
            EvolutionModule evolutionData)
        {
            var perksFromSave = new List<IPerk>();
            foreach (var storedPerk in storedPerson.Perks)
            {
                var perkScheme = schemeService.GetScheme<IPerkScheme>(storedPerk.Sid);

                var perk = new Perk
                {
                    Scheme = perkScheme
                };

                if (storedPerk.Level != null)
                {
                    perk.CurrentLevel = new PerkLevel(storedPerk.Level.Value, storedPerk.SubLevel.Value);
                }

                // TODO Доработать, когда будет доработана прокачка больше, чем на один лвл
                var currentLevelScheme = perkScheme.Levels[0];

                perk.CurrentJobs = currentLevelScheme.Jobs.Select(job => new PerkJob(job)
                {
                    IsComplete = storedPerk.Jobs.Single(storedJob => storedJob.Type == job.Type && storedJob.Scope == job.Scope).IsComplete,
                    Progress = storedPerk.Jobs.Single(storedJob => storedJob.Type == job.Type && storedJob.Scope == job.Scope).Progress,
                }).ToArray();

                perksFromSave.Add(perk);
            }
            evolutionData.SetPerksForced(perksFromSave);
        }
    }
}
