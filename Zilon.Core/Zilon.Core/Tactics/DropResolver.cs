using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics
{
    //TODO Написать/перенести тесты
    public class DropResolver : IDropResolver
    {
        private readonly IDice _dice;
        private readonly ISchemeService _schemeService;
        private readonly IPropFactory _propFactory;

        public DropResolver(IDice dice, ISchemeService schemeService, IPropFactory propFactory)
        {
            _dice = dice;
            _schemeService = schemeService;
            _propFactory = propFactory;
        }

        public IProp[] GetProps(IEnumerable<DropTableScheme> dropTables)
        {
            var materializedDropTables = dropTables.ToArray();
            var props = GenerateContent(materializedDropTables);
            return props;
        }

        private IProp[] GenerateContent(DropTableScheme[] dropTables)
        {
            //TODO Модификаторы нужно будет получать из игрока, актёра, который открыл сундук и актёров, которые на это могу повлиять.
            var modificators = new TrophyTableModificatorcs[0];
            var rolledRecords = new List<DropTableRecordSubScheme>();

            foreach (var table in dropTables)
            {
                var records = table.Records;
                var recMods = GetModRecords(records, modificators);

                var totalWeight = recMods.Sum(x => x.Weight);

                // Записи с нулевым весом всегда добавляются.
                rolledRecords.AddRange(records.Where(x => x.Weight == 0));

                for (var rollIndex = 0; rollIndex < table.Rolls; rollIndex++)
                {
                    var roll = _dice.Roll(totalWeight);
                    var recMod = TrophyRoller.GetRecord(recMods, roll);

                    if (recMod.Record.SchemeSid == null)
                        continue;

                    rolledRecords.Add(recMod.Record);
                }
            }

            //TODO Разобрать эту конструкцию на более примитивные.
            var props = rolledRecords.Select(x => GenerateTrophyFromTable(x, _dice, _schemeService)).ToArray();

            return props;
        }

        private TrophyTableRecordMod[] GetModRecords(IEnumerable<DropTableRecordSubScheme> records,
            IEnumerable<TrophyTableModificatorcs> modificators)
        {
            var resultList = new List<TrophyTableRecordMod>();
            foreach (var record in records)
            {
                if (record.SchemeSid == null)
                {
                    resultList.Add(new TrophyTableRecordMod
                    {
                        Record = record,
                        Weight = record.Weight
                    });
                    continue;
                }

                var recordModificators = modificators.Where(x => x.PropSids == null || x.PropSids.Contains(record.SchemeSid));
                var totalWeightBonus = recordModificators.Sum(x => x.Bonus);
                resultList.Add(new TrophyTableRecordMod
                {
                    Record = record,
                    Weight = (int)Math.Round(record.Weight + record.Weight * totalWeightBonus)
                });
            }

            return resultList.ToArray();
        }

        private IProp GenerateTrophyFromTable(DropTableRecordSubScheme record, IDice dice, ISchemeService schemeService)
        {
            var scheme = schemeService.GetScheme<PropScheme>(record.SchemeSid);
            var propClass = GetPropClass(scheme);

            switch (propClass)
            {
                case PropClass.Equipment:
                    //TODO Вынести в отдельный метод. Чтобы можно было использовать в крафте.
                    var powerRange = record.MaxPower - record.MinPower;
                    var power = dice.Roll(powerRange) + record.MinPower;

                    //TODO Вернуть, когда для экипировки будут бонусы
                    //var bonusPowers = new Dictionary<PropBonusType, int>();
                    //if (scheme.Bonuses != null)
                    //{
                    //    foreach (var bonus in scheme.Bonuses)
                    //    {
                    //        var bonusPower = dice.Range(record.MinPower, record.MaxPower);
                    //        bonusPowers.Add(bonus.Type, bonusPower);
                    //    }
                    //}

                    var equipment = _propFactory.CreateEquipment(scheme);
                    equipment.Power = power;
                    return equipment;

                case PropClass.Resource:
                    var countRange = record.MaxCount - record.MinCount;
                    var rolledCount = _dice.Roll(countRange) + record.MinCount;
                    return new Resource(scheme, rolledCount);

                case PropClass.Concept:

                    var propScheme = _schemeService.GetScheme<PropScheme>(record.Concept);

                    return new Concept(scheme, propScheme);

                default:
                    throw new ArgumentException($"Неизвестный класс {propClass} объекта {scheme}.");
            }
        }

        private static PropClass GetPropClass(PropScheme scheme)
        {
            if (scheme.Equip != null)
                return PropClass.Equipment;

            if (scheme.Sid == "conceptual-scheme")
                return PropClass.Concept;

            return PropClass.Resource;
        }

        enum PropClass
        {
            Equipment = 1,
            Resource = 2,
            Concept = 3
        }
    }
}
