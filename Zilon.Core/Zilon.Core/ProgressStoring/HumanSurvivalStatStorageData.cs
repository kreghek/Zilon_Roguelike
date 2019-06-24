using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.Persons;

namespace Zilon.Core.ProgressStoring
{
    public sealed class HumanSurvivalStatStorageData
    {
        public SurvivalStatType Type { get; set; }
        public int Value { get; set; }
    }
}
