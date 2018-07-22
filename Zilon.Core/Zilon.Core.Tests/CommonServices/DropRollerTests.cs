using FluentAssertions;

using NUnit.Framework;

using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.CommonServices.Tests
{
    [TestFixture]
    public class DropRollerTests
    {
        /// <summary>
        /// 1. В системе есть две записи дропа с весами 16 и 64. Ролл на 16.
        /// 2. Рассчитываем дроп.
        /// 3. Получаем дроп из первой записи.
        /// </summary>
        [Test]
        public void GetRecord_RollIn1stBorder_Has1stRecord()
        {
            // ARRANGE
            var records = new[] {
                new DropTableRecordSubScheme{
                    SchemeSid = "trophy1",
                    Weight = 16,
                },
                new DropTableRecordSubScheme{
                    SchemeSid = "trophy2",
                    Weight = 64,
                }
            };

            var roll = 16;

            var recMods = records.Select(x => new DropTableModRecord
            {
                Record = x,
                ModifiedWeight = x.Weight
            }).ToArray();



            // ACT
            var recordMod = DropRoller.GetRecord(recMods, roll);



            // ASSERT
            recordMod.Record.SchemeSid.Should().Be("trophy1");
        }
    }
}