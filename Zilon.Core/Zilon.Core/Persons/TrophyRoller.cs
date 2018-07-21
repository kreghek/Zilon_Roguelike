using System;

namespace Zilon.Core.Persons
{
    //TODO Вынести в хелперы или сервисы
    public static class TrophyRoller
    {
        public static TrophyTableRecordMod GetRecord(TrophyTableRecordMod[] records, int roll)
        {
            var pointer = 1;

            foreach (var record in records)
            {
                if (roll >= pointer && roll <= pointer + record.Weight - 1)
                {
                    return record;
                }

                pointer += record.Weight;
            }

            throw new Exception();
        }
    }
}
