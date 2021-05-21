using System;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.CommonServices;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.CommonServices
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
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
            var records = new[]
            {
                new TestDropTableRecordSubScheme("trophy1", 16),
                new TestDropTableRecordSubScheme("trophy2", 64)
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

        private sealed class TestDropTableRecordSubScheme : IDropTableRecordSubScheme
        {
            public TestDropTableRecordSubScheme(string schemeSid, int weight)
            {
                SchemeSid = schemeSid ?? throw new ArgumentNullException(nameof(schemeSid));

                if (weight <= 0)
                {
                    throw new ArgumentNullException(nameof(weight),
                        "Вес записи в таблице дропа должен быть положительным.");
                }

                Weight = weight;
            }

            public IDropTableScheme[] Extra { get; }
            public int MaxCount { get; }
            public int MinCount { get; }
            public string SchemeSid { get; }
            public int Weight { get; }
        }
    }
}