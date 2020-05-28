using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.PersonGeneration;
using Zilon.Core.Persons;

namespace Zilon.Emulation.Common
{
    /// <summary>
    /// Вспомогательный класс для генерации персонажа в окружениях для тестирования.
    /// </summary>
    public static class PersonCreateHelper
    {
        /// <summary>
        /// Создаёт персонажа.
        /// </summary>
        /// <param name="serviceProvider"> Контейнер DI, откуда извлекаются сервисы для создания персонажа. </param>
        /// <returns> Возвращает созданного персонажа. </returns>
        public static IPerson CreateStartPerson(IServiceProvider serviceProvider, IFraction fraction)
        {
            var personFactory = serviceProvider.GetRequiredService<IPersonFactory>();

            var startPerson = personFactory.Create("human-person", fraction);
            return startPerson;
        }
    }
}
