using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics
{
    public class DropResolver : IDropResolver
    {
        private readonly IDropResolverRandomSource _randomSource;
        private readonly ISchemeService _schemeService;
        private readonly IPropFactory _propFactory;

        public DropResolver(IDropResolverRandomSource randomSource, ISchemeService schemeService, IPropFactory propFactory)
        {
            _randomSource = randomSource;
            _schemeService = schemeService;
            _propFactory = propFactory;
        }

        public IProp[] Resolve(IEnumerable<IDropTableScheme> dropTables)
        {
            var materializedDropTables = dropTables.ToArray();
            var props = ResolveInner(materializedDropTables);
            return props;
        }

        private IProp[] ResolveInner(IDropTableScheme[] dropTables)
        {
            var modificators = new IDropTableModificatorScheme[0];
            var rolledRecords = new List<IDropTableRecordSubScheme>();

            var openDropTables = new List<IDropTableScheme>(dropTables);
            while(openDropTables.Any())
            {
                var table = openDropTables[0];
                var records = table.Records;
                var recMods = GetModRecords(records, modificators);

                var totalWeight = recMods.Sum(x => x.ModifiedWeight);

                for (var rollIndex = 0; rollIndex < table.Rolls; rollIndex++)
                {
                    var rolledWeight = _randomSource.RollWeight(totalWeight);
                    var recMod = DropRoller.GetRecord(recMods, rolledWeight);

                    if (recMod.Record.SchemeSid == null)
                    {
                        continue;
                    }

                    rolledRecords.Add(recMod.Record);

                    if (recMod.Record.Extra != null)
                    {
                        openDropTables.AddRange(recMod.Record.Extra);
                    }
                }

                openDropTables.RemoveAt(0);
            }

            var props = rolledRecords.Select(GenerateProp).ToArray();

            return props;
        }

        private DropTableModRecord[] GetModRecords(IEnumerable<IDropTableRecordSubScheme> records,
            IEnumerable<IDropTableModificatorScheme> modificators)
        {
            var modificatorsArray = modificators.ToArray();

            var resultList = new List<DropTableModRecord>();
            foreach (var record in records)
            {
                if (record.SchemeSid == null)
                {
                    resultList.Add(new DropTableModRecord
                    {
                        Record = record,
                        ModifiedWeight = record.Weight
                    });
                    continue;
                }

                var recordModificators = modificatorsArray.Where(x => x.PropSids == null || x.PropSids.Contains(record.SchemeSid));
                var totalWeightMultiplier = recordModificators.Sum(x => x.WeightBonus) + 1;
                resultList.Add(new DropTableModRecord
                {
                    Record = record,
                    ModifiedWeight = (int)Math.Round(record.Weight * totalWeightMultiplier)
                });
            }

            return resultList.ToArray();
        }

        private IProp GenerateProp(IDropTableRecordSubScheme record)
        {
            var scheme = _schemeService.GetScheme<IPropScheme>(record.SchemeSid);
            var propClass = GetPropClass(scheme);

            switch (propClass)
            {
                case PropClass.Equipment:
                    var equipment = _propFactory.CreateEquipment(scheme);
                    return equipment;

                case PropClass.Resource:
                    var rolledCount = _randomSource.RollResourceCount(record.MinCount, record.MaxCount);
                    var resource = _propFactory.CreateResource(scheme, rolledCount);
                    return resource;

                case PropClass.Concept:

                    throw new ArgumentException($"Пока концепты не поддерживаются.");

                    var propScheme = _schemeService.GetScheme<IPropScheme>("record.Concept");

                    return new Concept(scheme, propScheme);

                default:
                    throw new ArgumentException($"Неизвестный класс {propClass} объекта {scheme}.");
            }
        }

        private static PropClass GetPropClass(IPropScheme scheme)
        {
            if (scheme.Equip != null)
            {
                return PropClass.Equipment;
            }

            if (scheme.Sid == "conceptual-scheme")
            {
                return PropClass.Concept;
            }

            return PropClass.Resource;
        }

        private enum PropClass
        {
            Equipment = 1,
            Resource = 2,
            Concept = 3
        }
    }
}
