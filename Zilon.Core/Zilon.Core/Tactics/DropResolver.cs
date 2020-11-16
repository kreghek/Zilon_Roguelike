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
        private readonly IPropFactory _propFactory;
        private readonly IDropResolverRandomSource _randomSource;
        private readonly ISchemeService _schemeService;
        private readonly IUserTimeProvider _userTimeProvider;

        public DropResolver(
            IDropResolverRandomSource randomSource,
            ISchemeService schemeService,
            IPropFactory propFactory,
            IUserTimeProvider userTimeProvider)
        {
            _randomSource = randomSource;
            _schemeService = schemeService;
            _propFactory = propFactory;
            _userTimeProvider = userTimeProvider;
        }

        private void AddEvilHourModifiers(IList<IDropTableModificatorScheme> totalModifierList)
        {
            const float PRE_DAY_COUNT = 5.0f;
            const float POST_DAY_COUNT = 2.0f;

            var currentDate = _userTimeProvider.GetCurrentTime();

            // Основано на хелловине
            var evilHour = new DateTime(currentDate.Year, 11, 2);
            var evilHourStart = evilHour.AddDays(-PRE_DAY_COUNT);
            var evilHourEnd = evilHour.AddDays(POST_DAY_COUNT);
            if ((evilHourStart <= currentDate) && (currentDate <= evilHourEnd))
            {
                IDropTableModificatorScheme mod;
                if (currentDate <= evilHour)
                {
                    // Канун злого часа
                    mod = CreateEvilHourModifier(evilHour, PRE_DAY_COUNT, currentDate);
                }
                else
                {
                    // Хвост события
                    mod = CreateEvilHourModifier(currentDate, POST_DAY_COUNT, evilHourEnd);
                }

                totalModifierList.Add(mod);
            }
        }

        private static IDropTableModificatorScheme CreateEvilHourModifier(
            DateTime targetDate,
            float duration,
            DateTime currentDate)
        {
            var dateDiff = targetDate - currentDate;

            var days = dateDiff.Days;
            var ration = days / duration;
            var inversedRation = 1 - ration;
            var bonus = duration * inversedRation;
            var mod = new DropTableModificatorScheme
            {
                PropSids = new[]
                {
                    "evil-pumpkin"
                },

                //TODO Зачем вообще здесь -1. Бонус - это число, на которое нужно умножить.
                WeightBonus = bonus - 1
            };
            return mod;
        }

        private IProp GenerateProp(IDropTableRecordSubScheme record)
        {
            try
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

                        var propScheme = _schemeService.GetScheme<IPropScheme>("record.Concept");

                        return new Concept(scheme, propScheme);

                    default:
                        throw new ArgumentException($"Неизвестный класс {propClass} объекта {scheme}.");
                }
            }
            catch (Exception exception)
            {
                //TODO Оборачивать в доменное исключение. Создать собственный тип.
                throw new Exception($"Ошибка при обработке записи дропа {record.SchemeSid}", exception);
            }
        }

        private IDropTableModificatorScheme[] GetModifiers()
        {
            var totalModifierList = new List<IDropTableModificatorScheme>();
            AddEvilHourModifiers(totalModifierList);

            return totalModifierList.ToArray();
        }

        private static DropTableModRecord[] GetModRecords(
            IEnumerable<IDropTableRecordSubScheme> records,
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

                var recordModificators =
                    modificatorsArray.Where(x => (x.PropSids == null) || x.PropSids.Contains(record.SchemeSid));
                var totalWeightMultiplier = recordModificators.Sum(x => x.WeightBonus) + 1;
                resultList.Add(new DropTableModRecord
                {
                    Record = record,
                    ModifiedWeight = (int)Math.Round(record.Weight * totalWeightMultiplier)
                });
            }

            return resultList.ToArray();
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

        private IProp[] ResolveInner(IDropTableScheme[] dropTables)
        {
            var modificators = GetModifiers();
            var rolledRecords = new List<IDropTableRecordSubScheme>();

            var openDropTables = new List<IDropTableScheme>(dropTables);
            while (openDropTables.Any())
            {
                try
                {
                    var table = openDropTables[0];
                    var records = table.Records;
                    if (!records.Any())
                    {
                        // Do not try to roll if drop table has no records.

                        // Dont forget to remove empty drop table from open to avoid endless loop.
                        openDropTables.RemoveAt(0);
                        continue;
                    }

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
                            //TODO Доделать учёт Rolls для экстра.
                            // Сейчас все экстра гарантированно выпадают по разу.
                            openDropTables.AddRange(recMod.Record.Extra);
                        }
                    }

                    openDropTables.RemoveAt(0);
                }
                catch
                {
                    openDropTables.RemoveAt(0);

                    //TODO FIX
                }
            }

            var props = rolledRecords.Select(GenerateProp)
                                     .ToArray();

            return props;
        }

        public IProp[] Resolve(IEnumerable<IDropTableScheme> dropTables)
        {
            var materializedDropTables = dropTables.ToArray();
            var props = ResolveInner(materializedDropTables);
            return props;
        }

        private enum PropClass
        {
            Equipment = 1,
            Resource = 2,
            Concept = 3
        }
    }
}