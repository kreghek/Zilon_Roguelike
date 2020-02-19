using System;
using System.Threading.Tasks;

using Zilon.Core.Tactics;

namespace Zilon.Core.MassSectorGenerator
{
    /// <summary>
    /// Валидатор сектора.
    /// </summary>
    public interface ISectorValidator
    {
        /// <summary>
        /// Выполнение валидации.
        /// </summary>
        /// <param name="sector"> Проверяемый сектор. </param>
        /// <param name="scopeContainer"> Контейнер с зависимостями. </param>
        Task Validate(ISector sector, IServiceProvider scopeContainer);
    }
}
