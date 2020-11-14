using System;
using Zilon.Core.Persons;

namespace Zilon.Core.CommonServices
{
    /// <summary>
    /// Вспомогательный сервис для выборки записей из таблицы дропа по броску.
    /// </summary>
    public static class DropRoller
    {
        /// <summary>
        /// Возвращает запись таблицы дропа по указанному значению броска.
        /// </summary>
        /// <param name="records"> Записи таблицы дропа с учётом модификаторов. </param>
        /// <param name="roll"> Бросок куба. </param>
        /// <returns> Запись из таблицы дропа. </returns>
        public static DropTableModRecord GetRecord(DropTableModRecord[] records, int roll)
        {
            if (roll == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(roll),
                    "Результат выбора записи в таблице дропа не может быть 0.");
            }

            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            var pointer = 1;

            foreach (var record in records)
            {
                if (roll >= pointer && roll <= pointer + record.ModifiedWeight - 1)
                {
                    return record;
                }

                pointer += record.ModifiedWeight;
            }

            throw new ArgumentOutOfRangeException(nameof(roll),
                "Результат выбора записи в таблице дропа больше, чем суммарный вес таблицы дропа.");
        }
    }
}