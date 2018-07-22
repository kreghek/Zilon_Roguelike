using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zilon.Core.Persons
{
    //TODO Исправить опечатку
    /// <summary>
    /// Модификатор таблицы дропа.
    /// </summary>
    public sealed class TrophyTableModificatorcs
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] PropSids { get; set; }
        public float Bonus { get; set; }
    }
}
